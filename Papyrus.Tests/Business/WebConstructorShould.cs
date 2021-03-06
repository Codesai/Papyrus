﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Papyrus.Business.Exporters;
using Papyrus.Business.Products;
using Papyrus.Business.Topics;

namespace Papyrus.Tests.Business {
    [TestFixture]
    public class WebConstructorShould {
        private const string EnglishContent = "Content";
        private const string EnglishTitle = "Title";
        private TopicQueryRepository topicRepo;
        private ProductRepository productRepo;
        private WebsiteConstructor websiteConstructor;
        private string Spanish = "es-ES";
        private string English = "en-GB";
        private string LastVersionName = "15.11.27";
        private readonly ExportableDocument englishDocument = new ExportableDocument(EnglishTitle, EnglishContent);

        [SetUp]
        public void SetUp() {
            topicRepo = Substitute.For<TopicQueryRepository>();
            productRepo = Substitute.For<ProductRepository>();
            websiteConstructor = new WebsiteConstructor(topicRepo);
        }
        
        [Test]
        public async Task construct_a_website_with_documents_associated_to_given_product_and_version() {
            var versionsNames = VersionNames(LastVersionName);
            var opportunity = new Product("OpportunityId", "Opportunity", VersionsFrom(versionsNames));
            topicRepo.GetAllDocumentsFor(opportunity.Id, LastVersionName, Spanish)
                .Returns(AsyncDocumentsList(englishDocument));

            var websites = await websiteConstructor.Construct(opportunity, Languages(Spanish));

            var websiteDocument = websites.First().Documents.First();
            websiteDocument.Content.Should().Be(EnglishContent);
            websiteDocument.Title.Should().Be(EnglishTitle);
        }

        [Test]
        public async Task not_create_website_when_there_are_no_document_available() {
            var versionsNames = VersionNames(LastVersionName);
            var opportunity = new Product("OpportunityId", "Opportunity", VersionsFrom(versionsNames));
            topicRepo.GetAllDocumentsFor(opportunity.Id, LastVersionName, Spanish)
                .Returns(Task.FromResult(new List<ExportableDocument>()));

            var websites = await websiteConstructor.Construct(opportunity, Languages(Spanish));

            websites.Should().BeEmpty();
        }
        
        [Test]
        public async Task fill_product_name() {
            var versionsNames = VersionNames(LastVersionName);
            var papyrus = new Product("PapyrusId", "Papyrus", VersionsFrom(versionsNames));
            topicRepo.GetAllDocumentsFor(papyrus.Id, LastVersionName, English)
                .Returns(AsyncDocumentsList(englishDocument));

            var websites = await websiteConstructor.Construct(papyrus, Languages(English));

            websites.First().ProductName.Should().Be(papyrus.Name);
        }

        private List<ProductVersion> VersionsFrom(List<string> versionsNames) {
            return versionsNames.Select(versionsName => 
                    new ProductVersion(versionsName, versionsName, DateTime.Today))
                .ToList();
        }

        private static async Task<List<ExportableDocument>> AsyncDocumentsList(ExportableDocument document) {
            return new List<ExportableDocument>{ document };
        }

        private List<string> Languages(string language) {
            return new List<string> { language };
        }

        private static List<string> VersionNames(params string[] languages) {
            return languages.ToList();
        }

        private static List<Product> ProductsList(params Product[] products) {
            return products.ToList();
        }
    }
}