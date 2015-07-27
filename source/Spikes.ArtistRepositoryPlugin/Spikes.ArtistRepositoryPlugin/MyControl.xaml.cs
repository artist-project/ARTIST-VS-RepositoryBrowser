using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Spikes.Spikes_ArtistRepositoryPlugin.WebserviceClient;
using Task = System.Threading.Tasks.Task;

namespace Spikes.Spikes_ArtistRepositoryPlugin
{
    /// <summary>
    /// Interaction logic for MyControl.xaml
    /// </summary>
    public partial class MyControl : UserControl
    {
        private ArtistRepositoryClient _client = new ArtistRepositoryClient();
        
        public MyControl()
        {
            InitializeComponent();

            // Cleanup previous temp folder
            var folder = GetTempFolder();
            if (Directory.Exists(folder))
            {
                try
                {
                    var files = Directory.GetFiles(folder);
                    foreach (var file in files)
                    {
                        File.Delete(file);
                    }
                }
                catch (Exception)
                {}
            }

            // Make sure temp folder exists
            Directory.CreateDirectory(folder);
        }

        private static string GetTempFolder()
        {
            return Path.Combine(Path.GetTempPath(), "ArtistRepositoryBrowser");
        }

        private async void ProjectsTreeView_Loaded(object sender, RoutedEventArgs e)
        {
            var treeView = sender as TreeView;
            await RefreshProjectTree(treeView);
        }

        private async Task RefreshProjectTree(TreeView treeView)
        {
            var projects = (await _client.GetProjectsAsync()).ToList();


            // fetch all artefacts
            var artefacts = new Dictionary<string, List<ArArtifact>>();
            foreach (var project in projects)
            {
                if (project.packages != null)
                {
                    var artefactsForProject = (await _client.GetArtifactsAsync(project.id)).ToList();
                    artefacts.Add(project.id, artefactsForProject);
                }
            }

            // Clear and rebuild after all is fetched
            treeView.Items.Clear();

            foreach (var project in projects)
            {
                var projectItem = new TreeViewItem()
                {
                    Header = project.id,
                    IsExpanded = true,
                    Tag = project
                };

                if (project.packages != null)
                {
                    var artefactsForProject = artefacts[project.id];

                    foreach (var package in project.packages)
                    {
                        var packageItem = new TreeViewItem()
                        {
                            Header = package.label,
                            IsExpanded = true,
                            Tag = package
                        };

                        ArPackage package1 = package;
                        foreach (var artefact in artefactsForProject.Where(_ => string.Equals(_.packageId, package1.id)))
                        {
                            var artefactItem = new TreeViewItem()
                            {
                                Header = string.IsNullOrEmpty(artefact.label) ? "#no-label#" : artefact.label,
                                IsExpanded = false,
                                Tag = artefact
                            };
                            packageItem.Items.Add(artefactItem);
                        }

                        projectItem.Items.Add(packageItem);
                    }
                }

                treeView.Items.Add(projectItem);
            }
        }

        private Point _startPoint;
        private bool _isDragging;

        private async void ProjectsTreeView_MouseMove(object sender, MouseEventArgs e)
        {
            // Get the current mouse position
            Point mousePos = e.GetPosition(null);
            Vector diff = _startPoint - mousePos;

            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                 Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                if (_isDragging)
                    return;

                var treeView = ProjectsTreeView;
                var selectedItem = treeView.SelectedItem as TreeViewItem;
                if (selectedItem == null)
                    return;
                if (!(selectedItem.Tag is ArArtifact))
                    return;

                var artefact = selectedItem.Tag as ArArtifact;

                _isDragging = true;


                // Prepare temp folder
                // TODO: cache & subfolders foreach project?
                var folder = GetTempFolder();

                // Create temp file
                var content = await _client.GetArtefactContentAsync(artefact.project, artefact.id);
                var filename = Path.Combine(folder, artefact.label);
                File.WriteAllText(filename, content);

                // Start drag of temp file
                var dragData = new DataObject(DataFormats.FileDrop, new[] { filename });
                DragDrop.DoDragDrop((DependencyObject) e.Source, dragData, DragDropEffects.Copy);
            }
            else
            {
                _startPoint = e.GetPosition(null);
                _isDragging = false;
            }
        }

        private ArProject _currentProjectDropTarget = null;
        private ArPackage _currentPackageDropTarget = null;
        private string _currentDropFilename = null;

        private void ClearCurrentDrop()
        {
            _currentProjectDropTarget = null;
            _currentPackageDropTarget = null;
            _currentDropFilename = null;
        }

        private void SetCurrentDrop(TreeViewItem packageTreeViewItem, string filename)
        {
            packageTreeViewItem.IsSelected = true;

            var package = packageTreeViewItem.Tag as ArPackage;
            Debug.Assert(package != null);

            var projectTreeViewItem = GetParentItem<ArProject>(packageTreeViewItem);
            Debug.Assert(projectTreeViewItem != null);

            var project = projectTreeViewItem.Tag as ArProject;
            Debug.Assert(project != null);

            _currentProjectDropTarget = project;
            _currentPackageDropTarget = package;
            _currentDropFilename = filename;
        }

        private async void ProjectsTreeView_Drop(object sender, DragEventArgs e)
        {
            e.Handled = true;

            // Should not be possible... (just in case)
            if (_currentPackageDropTarget == null || _currentProjectDropTarget == null || _currentDropFilename == null)
                return;

            if (!File.Exists(_currentDropFilename))
                return;

            var artefactFilename = Path.GetFileNameWithoutExtension(_currentDropFilename);
            if (string.IsNullOrEmpty(artefactFilename))
                artefactFilename = Path.GetFileName(_currentDropFilename);
            var cleanArtefactId = artefactFilename.Replace(' ', '_');
            var artefactId = string.Format("{0}:{1}", _currentPackageDropTarget.id, cleanArtefactId);
            var artefactBody = File.ReadAllText(_currentDropFilename);

            await _client.UploadArtefactAsync(
                _currentProjectDropTarget.id,
                _currentPackageDropTarget.id,
                artefactId,
                artefactFilename,
                artefactBody);

            ClearCurrentDrop();

            await RefreshProjectTree(ProjectsTreeView);
        }

        private void ProjectsTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SetDetail();
            SetContextMenu();
        }

        private void SetContextMenu()
        {
            var treeView = ProjectsTreeView;
            var selectedItem = treeView.SelectedItem as TreeViewItem;
            if (selectedItem == null)
                return;

            if (selectedItem.Tag is ArArtifact)
            {
                treeView.ContextMenu = treeView.Resources["ArtefactContext"] as ContextMenu;
            }
            else
            {
                treeView.ContextMenu = null;
            }
        }

        private void SetDetail()
        {
            this.LblDetailLabel.Text = null;
            this.LblDetailDescription.Text = null;

            var treeView = ProjectsTreeView;
            var selectedItem = treeView.SelectedItem as TreeViewItem;
            if (selectedItem == null)
                return;

            if (selectedItem.Tag is ArProject)
            {
                var project = selectedItem.Tag as ArProject;

                this.LblDetailLabel.Text = project.label;
                this.LblDetailDescription.Text = project.description;
            }
            else if (selectedItem.Tag is ArPackage)
            {
                var package = selectedItem.Tag as ArPackage;

                this.LblDetailLabel.Text = package.label;
                //this.LblDetailDescription.Text = project.description;
            }
            else if (selectedItem.Tag is ArArtifact)
            {
                var artefact = selectedItem.Tag as ArArtifact;

                this.LblDetailLabel.Text = artefact.label;
                this.LblDetailDescription.Text = artefact.description;
            }
        }

        private void ProjectsTreeView_DragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
            
            var filename = GetFilename(e.Data);

            if (_isDragging)
            {
                // Check if we are really dragging
                if (string.Equals(filename, _currentDropFilename))
                {
                    e.Effects = DragDropEffects.None;
                    return;
                }

                // Started a new drag from outside the control
                _isDragging = false;
                ClearCurrentDrop();
            }

            if (string.IsNullOrEmpty(filename))
            {
                e.Effects = DragDropEffects.None;
                return;
            }

            Point mousePos = e.GetPosition(ProjectsTreeView);
            IInputElement dropElement = ProjectsTreeView.InputHitTest(mousePos);
            var dropTreeItem = GetParentItem<ArPackage>(dropElement as DependencyObject);

            if (dropTreeItem == null)
            {
                e.Effects = DragDropEffects.None;
                return;
            }

            e.Effects = DragDropEffects.Copy;

            SetCurrentDrop(dropTreeItem, filename);
        }

        private static string GetFilename(IDataObject dragData)
        {
            if (dragData.GetDataPresent(DataFormats.FileDrop))
            {
                var filenames = dragData.GetData(DataFormats.FileDrop) as string[];
                if (filenames != null && filenames.Length == 1)
                {
                    var filename = filenames[0];
                    if (File.Exists(filename))
                    {
                        return filename;
                    }
                }
            }
            else if (dragData.GetDataPresent(DataFormats.Text))
            {
                var filename = dragData.GetData(DataFormats.Text) as string;
                if (File.Exists(filename))
                {
                    return filename;
                }
            }

            return null;
        }

        private static TreeViewItem GetParentItem<T>(DependencyObject dependencyObject) 
            where T : class
        {
            if (dependencyObject == null)
                return null;

            if (dependencyObject is TreeViewItem)
            {
                var treeViewItem = dependencyObject as TreeViewItem;

                if (treeViewItem.Tag is T)
                    return treeViewItem;
            }

            var parentObject = VisualTreeHelper.GetParent(dependencyObject);
            return GetParentItem<T>(parentObject);
        }




        private bool GetSelected(ref ArProject project, ref ArPackage package, ref ArArtifact artefact)
        {
            var treeView = ProjectsTreeView;
            var selectedItem = treeView.SelectedItem as TreeViewItem;
            if (selectedItem == null)
                return false;

            var artefactItem = GetParentItem<ArArtifact>(selectedItem);
            if (artefactItem != null)
                artefact = artefactItem.Tag as ArArtifact;

            var packageItem = GetParentItem<ArPackage>(selectedItem);
            if (packageItem != null)
                package = packageItem.Tag as ArPackage;

            var projectItem = GetParentItem<ArProject>(selectedItem);
            if (projectItem != null)
                project = projectItem.Tag as ArProject;

            return artefact != null || package != null || project != null;
        }

        private async void Artefact_MenuItem_Properties_Click(object sender, RoutedEventArgs e)
        {
            ArProject project = null;
            ArPackage package = null;
            ArArtifact artefact = null;

            if (!GetSelected(ref project, ref package, ref artefact))
                return;

            if (artefact == null || package == null || project == null)
                return;

            var serverArtefact = await _client.GetArtifactAsync(project.id, artefact.id);

            var propertiesDialog = new ArtefactPropertyDialog(project.id, serverArtefact);
            if (propertiesDialog.ShowDialog() == true)
            {
                await _client.UpdateArtefactAsync(project.id, serverArtefact);
                await RefreshProjectTree(ProjectsTreeView);
            }
       }

        private async void Artefact_MenuItem_Delete_Click(object sender, RoutedEventArgs e)
        {
            // TODO: "are you sure? - popup"

            ArProject project = null;
            ArPackage package = null;
            ArArtifact artefact = null;

            if (!GetSelected(ref project, ref package, ref artefact))
                return;

            if (artefact == null || package == null || project == null)
                return;

            await _client.DeleteArtefact(project.id, artefact);
            await RefreshProjectTree(ProjectsTreeView);
        }
    }
}