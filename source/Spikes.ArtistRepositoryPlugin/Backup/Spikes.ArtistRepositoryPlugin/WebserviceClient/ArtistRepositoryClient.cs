using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Spikes.Spikes_ArtistRepositoryPlugin.WebserviceClient
{
    public class ArtistRepositoryClient : BaseWebService
    {
        private const string _serviceHost = @"http://artist-demo.iao.fraunhofer.de/";
        private const string _sslServiceHost = @"https://artist-demo.iao.fraunhofer.de/";

        private const string _username = "demo@test.artist-demo.eu";
        private const string _password = "demo123";
        private const string _basicAuthorizationToken = "ajBnZmZFOXVfVHFpSGJmN3MzWkc5WUVCSWk4YTpWYkJfSm9fdDNwSXN5SFB1WG9iU0pHbjVwTm9h";

        private OAuthToken _token = null;
        private DateTime _tokenTimestamp;

        private async Task<string> _GetAccessTokenAsync()
        {
            if (_token == null)
            {
                // Get token
                _token = await _GetTokenAsync();
                _tokenTimestamp = DateTime.Now;
            }
            else
            {
                // Check expires_in with margin on 60 secs
                if ((DateTime.Now - _tokenTimestamp).TotalSeconds > _token.expires_in - 60)
                {
                    // Do refresh
                    _token = await _RefreshTokenAsync(_token.refresh_token);
                }
            }

            return "Bearer " + _token.access_token;
        }

        private async Task<OAuthToken> _GetTokenAsync()
        {
            string url = string.Format("oauth2/token?grant_type=password&username={0}&password={1}", _username, _password);
            var result = await DoPostWebRequest<OAuthToken>(_sslServiceHost + url, null, "Basic " + _basicAuthorizationToken, UrlEncodedContentType);

            return result;
        }

        private async Task<OAuthToken> _RefreshTokenAsync(string refreshToken)
        {
            string url = string.Format("oauth2/token?grant_type=refresh_token&refresh_token={0}", refreshToken);
            var result = await DoPostWebRequest<OAuthToken>(_sslServiceHost + url, null, "Basic " + _basicAuthorizationToken, UrlEncodedContentType);

            return result;
        }

        public async Task<IEnumerable<ArProject>> GetProjectsAsync()
        {
            string url = string.Format("api/projects");
            var result = await DoWebRequest<List<ArProject>>(_serviceHost + url, await _GetAccessTokenAsync());

            return result;
        }

        public async Task<IEnumerable<ArArtifact>> GetArtifactsAsync(string project)
        {
            string url = string.Format("api/artefacts/" + project);
            var result = await DoWebRequest<List<ArArtifact>>(_serviceHost + url, await _GetAccessTokenAsync());

            return result;
        }

        public async Task<string> GetArtefactContentAsync(string project, string artefact)
        {
            string url = string.Format("api/artefacts/{0}/{1}/content", project, artefact);
            var result = await DoWebRequestRaw(_serviceHost + url, await _GetAccessTokenAsync());

            return result;
        }

        internal async Task UploadArtefactAsync(string projectId, string packageId, string artefactId, string artefactLabel, string artefactBody)
        {
            ArArtifact serverArtefact = null;

            // Create new
            {
                string url = string.Format("api/artefacts/{0}", projectId);

                var artefact = new ArArtifact()
                {
                    id = artefactId,
                    packageId = packageId,
                    label = artefactLabel
                };

                serverArtefact = await DoPostWebRequest<ArArtifact>(
                    _serviceHost + url,
                    artefact,
                    await _GetAccessTokenAsync());
            }

            // Upload content
            await UploadArtefact(projectId, artefactBody, serverArtefact.id);
        }

        private async Task UploadArtefact(string projectId, string artefactBody, string artefactId)
        {
            {
                string url = string.Format("api/artefacts/{0}/{1}/content", projectId, artefactId);
                var result = await DoPostWebRequest<string>(
                    _serviceHost + url,
                    artefactBody,
                    await _GetAccessTokenAsync(),
                    BinaryContentType);
            }
        }

        public async Task<ArArtifact> GetArtifactAsync(string projectId, string artefactId)
        {
            string url = string.Format("api/artefacts/{0}/{1}", projectId, artefactId);
            var result = await DoWebRequest<ArArtifact>(_serviceHost + url, await _GetAccessTokenAsync());

            return result;
        }

        public async Task<ArArtifact> UpdateArtefactAsync(string projectId, ArArtifact artefact)
        {
            var previousContent = await GetArtefactContentAsync(projectId, artefact.id);

            string url = string.Format("api/artefacts/{0}/{1}", projectId, artefact.id);

            // This will reset the content
            // TO VERIFY: Bug?
            var serverArtefact = await DoPutWebRequest<ArArtifact>(
                _serviceHost + url,
                artefact,
                await _GetAccessTokenAsync());

            // upload content again
            await UploadArtefact(projectId, previousContent, serverArtefact.id);


            return serverArtefact;
        }

        public async Task DeleteArtefact(string projectId, ArArtifact artefact)
        {
            string url = string.Format("api/artefacts/{0}/{1}", projectId, artefact.id);

            var serverArtefact = await DoDeleteWebRequest<ArArtifact>(
              _serviceHost + url,
              await _GetAccessTokenAsync());
        }
    }
}
