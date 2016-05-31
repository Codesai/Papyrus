﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Papyrus.Business.Products;
using Papyrus.Infrastructure.Core;

namespace Papyrus.Business.Exporters {
    public class MkDocsExporter {
        private const string YmlFileName = "mkdocs.yml";
        private const string MarkDownExtension = ".md";
        private readonly string mkdocsTheme = "theme: readthedocs";
        private readonly string siteName = "site_name: SIMA Documentation";
        private static readonly string NewLine = System.Environment.NewLine;
        private readonly FileSystemImagesCopier imagesCopier;
        public const string IndexContent = "SIMA Documentation";
        private const string NewListItem = "- ";

        public MkDocsExporter(FileSystemImagesCopier imagesCopier) {
            this.imagesCopier = imagesCopier;
        }

        public virtual async Task Export(WebSite webSite, string path, string imagesFolder) {
            var configuration = new MkdocsConfiguration();
            await InitializeMkdocsStructure(path, configuration);
            var docsPath = Path.Combine(path, "docs");
            foreach (var document in webSite.Documents) {
                await ExportDocumentIn(document, docsPath);
                await GenerateDocInYml(document, Path.Combine(path, YmlFileName));
            }
            var imagesFolderDirectoryName = (new DirectoryInfo(imagesFolder)).Name;
            var newImagesDestination = Path.Combine(docsPath, imagesFolderDirectoryName);
            if (!Directory.Exists(newImagesDestination)) {
                imagesCopier.CopyFolder(imagesFolder, newImagesDestination);
            }
        }

        private async Task InitializeMkdocsStructure(string path, MkdocsConfiguration configuration) {
            var docsPath = Path.Combine(path, "docs");
            var docsDirectory = Directory.CreateDirectory(docsPath);
            await WriteInFile(Path.Combine(docsDirectory.FullName, "index.md"), IndexContent);
            await InitializeYmlFileIn(path, configuration);
        }

        private async Task InitializeYmlFileIn(string path, MkdocsConfiguration configuration) {
            if (File.Exists(Path.Combine(path, YmlFileName))) return;
            var ymlPath = Path.Combine(path, YmlFileName);

            configuration.Theme = "readthedocs";
            configuration.SiteName = "SIMA Documentation";
            configuration.AddPage("Home", "index.md");

            await WriteInFile(ymlPath, configuration.ToString());
//            await WriteInFile(ymlPath, mkdocsTheme);
//            await WriteInFile(ymlPath, siteName);
//            await WriteInFile(ymlPath, "pages:");
//            await WriteInFile(ymlPath, "- 'Home': 'index.md'");
        }

        private static async Task GenerateDocInYml(ExportableDocument document, string ymlPath) {
            var docReference = MkdocsPagePresentationFor(document);
            if (string.IsNullOrEmpty(document.Route)) {
                await WriteInFile(ymlPath, docReference);
                return;
            }
            if (!ReadContentOf(ymlPath).Contains(document.Route)) {
                await WriteInFile(ymlPath, NewListItemWith(document.Route));
            }
            await WriteInFile(ymlPath, "    " + docReference);
        }

        private static string ReadContentOf(string ymlPath) {
            return File.ReadAllText(ymlPath);
        }

        private static string NewListItemWith(string itemContent) {
            return NewListItem + "'" + itemContent + "':";
        }

        private static string MkdocsPagePresentationFor(ExportableDocument document) {
            return NewListItem + "'" + document.Title + "': '" + 
                Path.Combine(document.Route, ConvertToValidFileName(document.Title)) + MarkDownExtension + "'";
        }

        private static async Task ExportDocumentIn(ExportableDocument document, string directoryPath) {
            var documentDirectory = Directory.CreateDirectory(Path.Combine(directoryPath, document.Route));
            var documentPath = Path.Combine(documentDirectory.FullName, ConvertToValidFileName(document.Title) + MarkDownExtension);
            await WriteInFile(documentPath, document.Content);
        }

        private static async Task WriteInFile(string documentPath, string content) {
            await FileWriter.WriteFileWithContent(documentPath, content + NewLine);
        }

        private static string ConvertToValidFileName(string title) {
            return MkdocsFileNameConverter.ConvertToValidFileName(title);
        }
    }

    internal class MkdocsConfiguration {
        private string YmlPath { get; set; }
        public string Theme { get; set; }
        public string SiteName { get; set; }
        private readonly Dictionary<string, string> pages = new Dictionary<string, string>();  

        public void AddPage(string pageName, string fileName) {
            pages.Add(pageName, fileName);
        }

        public override string ToString() {
            var themeLine = "theme: " + Theme + Environment.NewLine;
            var siteNameLine = "site_name: " + SiteName + Environment.NewLine;
            var pagesLines = "pages:" + Environment.NewLine;
            foreach (var page in pages) {
                pagesLines += "- '" + page.Key + "': " + "'" + page.Value + "'"; 
            }
            return themeLine + siteNameLine + pagesLines;
        }
    }
}