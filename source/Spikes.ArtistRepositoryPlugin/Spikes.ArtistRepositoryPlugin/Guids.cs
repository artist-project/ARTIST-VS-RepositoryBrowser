// Guids.cs
// MUST match guids.h
using System;

namespace Spikes.Spikes_ArtistRepositoryPlugin
{
    static class GuidList
    {
        public const string guidSpikes_ArtistRepositoryPluginPkgString = "df778f5a-65cb-469c-8771-f42c2ea48ff3";
        public const string guidSpikes_ArtistRepositoryPluginCmdSetString = "ebe52bc7-0239-4b98-89e1-59369284fc03";
        public const string guidToolWindowPersistanceString = "7c6e668c-816f-40f0-a839-651407b689b0";

        public static readonly Guid guidSpikes_ArtistRepositoryPluginCmdSet = new Guid(guidSpikes_ArtistRepositoryPluginCmdSetString);
    };
}