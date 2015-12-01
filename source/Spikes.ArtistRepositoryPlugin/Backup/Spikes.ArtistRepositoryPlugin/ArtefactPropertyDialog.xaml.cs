using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Spikes.Spikes_ArtistRepositoryPlugin.WebserviceClient;

namespace Spikes.Spikes_ArtistRepositoryPlugin
{
    /// <summary>
    /// Interaction logic for UploadDialog.xaml
    /// </summary>
    public partial class ArtefactPropertyDialog : Window
    {
        private ArArtifact _artefact = null;

        public ArtefactPropertyDialog(string projectId, ArArtifact artefact)
        {
            InitializeComponent();

            _artefact = artefact;

            this.txtProjectAnswer.Text = projectId;
            this.txtPackageAnswer.Text = artefact.packageId;
            this.txtArtefactAnswer.Text = artefact.id;
            this.txtArtefactLabelAnswer.Text = artefact.label;
            this.txtArtefactDescriptionAnswer.Text = artefact.description;
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            _artefact.label = this.txtArtefactLabelAnswer.Text;
            _artefact.description = this.txtArtefactDescriptionAnswer.Text;

            this.DialogResult = true;
        }
    }
}
