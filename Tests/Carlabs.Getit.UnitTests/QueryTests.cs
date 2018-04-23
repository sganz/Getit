using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Dataflow;
using GraphQL.Common.Response;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Carlabs.Getit.UnitTests
{
    [TestClass]
    public class QueryTests
    {
        /// <summary>
        /// Model used to test data deserialization
        /// </summary>
        //        [QueryName("model")]
        //        class TestModel
        //        {
        //            public string Value;
        //        }

        [TestMethod]
        public void Select_StringList_AddsToQuery()
        {
            // Arrange

            IQueryStringBuilder queryStringBuilder = Substitute.For<IQueryStringBuilder>();
            IConfig config = Substitute.For<IConfig>();
            config.Url.Returns("http://www.somesite.com");
            Query query = new Query(queryStringBuilder, config);

            List<string> selectList = new List<string>()
            {
                "id",
                "name"
            };

            // Act
            query.Select(selectList);

            // Assert
            CollectionAssert.AreEqual(selectList, query.SelectList);
        }

        [TestMethod]
        public void From_String_AddsToQuery()
        {
            // Arrange
            IQueryStringBuilder queryStringBuilder = Substitute.For<IQueryStringBuilder>();
            IConfig config = Substitute.For<IConfig>();
            config.Url.Returns("http://www.somesite.com");
            Query query = new Query(queryStringBuilder, config);

            const string name = "user";

            // Act
            query.Name(name);

            // Assert
            Assert.AreEqual(name, query.QueryName);
        }

        [TestMethod]
        public void Select_String_AddsToQuery()
        {
            // Arrange
            IQueryStringBuilder queryStringBuilder = Substitute.For<IQueryStringBuilder>();
            IConfig config = Substitute.For<IConfig>();
            config.Url.Returns("http://www.somesite.com");
            Query query = new Query(queryStringBuilder, config);

            const string select = "id";

            // Act
            query.Select(select);

            // Assert
            Assert.AreEqual(select, query.SelectList.First());
        }

        [TestMethod]
        public void Select_DynamicArguments_AddsToQuery()
        {
            // Arrange
            IQueryStringBuilder queryStringBuilder = Substitute.For<IQueryStringBuilder>();
            IConfig config = Substitute.For<IConfig>();
            config.Url.Returns("http://www.somesite.com");
            Query query = new Query(queryStringBuilder, config);

            // Act
            query.Select("some", "thing", "else");

            // Assert
            List<string> shouldEqual = new List<string>()
            {
                "some",
                "thing",
                "else"
            };
            CollectionAssert.AreEqual(shouldEqual, query.SelectList);
        }

        [TestMethod]
        public void Select_ArrayOfString_AddsToQuery()
        {
            // Arrange
            IQueryStringBuilder queryStringBuilder = Substitute.For<IQueryStringBuilder>();
            IConfig config = Substitute.For<IConfig>();
            config.Url.Returns("http://www.somesite.com");
            Query query = new Query(queryStringBuilder, config);

            string[] selects =
            {
                "id",
                "name"
            };

            // Act
            query.Select(selects);

            // Assert
            List<string> shouldEqual = new List<string>()
            {
                "id",
                "name"
            };
            CollectionAssert.AreEqual(shouldEqual, query.SelectList);
        }

        [TestMethod]
        public void Select_ChainCombinationOfStringAndList_AddsToQuery()
        {
            // Arrange
            IQueryStringBuilder queryStringBuilder = Substitute.For<IQueryStringBuilder>();
            IConfig config = Substitute.For<IConfig>();
            config.Url.Returns("http://www.somesite.com");
            Query query = new Query(queryStringBuilder, config);

            const string select = "id";
            List<string> selectList = new List<string>()
            {
                "name",
                "email"
            };
            string[] selectStrings =
            {
                "array",
                "cool"
            };

            // Act
            query
                .Select(select)
                .Select(selectList)
                .Select("some", "thing", "else")
                .Select(selectStrings);

            // Assert
            List<string> shouldEqual = new List<string>()
            {
                "id",
                "name",
                "email",
                "some",
                "thing",
                "else",
                "array",
                "cool"
            };
            CollectionAssert.AreEqual(shouldEqual, query.SelectList);
        }

        [TestMethod]
        public void Where_IntegerArgumentWhere_AddsToWhere()
        {
            // Arrange
            IQueryStringBuilder queryStringBuilder = Substitute.For<IQueryStringBuilder>();
            IConfig config = Substitute.For<IConfig>();
            config.Url.Returns("http://www.somesite.com");
            Query query = new Query(queryStringBuilder, config);

            // Act
            query.Where("id", 1);

            // Assert
            Assert.AreEqual(1, query.WhereMap["id"]);
        }

        [TestMethod]
        public void Where_StringArgumentWhere_AddsToWhere()
        {
            // Arrange
            IQueryStringBuilder queryStringBuilder = Substitute.For<IQueryStringBuilder>();
            IConfig config = Substitute.For<IConfig>();
            config.Url.Returns("http://www.somesite.com");
            Query query = new Query(queryStringBuilder, config);

            // Act
            query.Where("name", "danny");

            // Assert
            Assert.AreEqual("danny", query.WhereMap["name"]);
        }

        [TestMethod]
        public void Where_DictionaryArgumentWhere_AddsToWhere()
        {
            // Arrange
            IQueryStringBuilder queryStringBuilder = Substitute.For<IQueryStringBuilder>();
            IConfig config = Substitute.For<IConfig>();
            config.Url.Returns("http://www.somesite.com");
            Query query = new Query(queryStringBuilder, config);

            Dictionary<string, int> dict = new Dictionary<string, int>()
            {
                {"from", 1},
                {"to", 100}
            };

            // Act
            query.Where("price", dict);

            // Assert
            Dictionary<string, int> queryWhere = (Dictionary<string, int>) query.WhereMap["price"];
            Assert.AreEqual(1, queryWhere["from"]);
            Assert.AreEqual(100, queryWhere["to"]);
            CollectionAssert.AreEqual(dict, (ICollection) query.WhereMap["price"]);
        }

        [TestMethod]
        public void Where_ChainedWhere_AddsToWhere()
        {
            // Arrange
            IQueryStringBuilder queryStringBuilder = Substitute.For<IQueryStringBuilder>();
            IConfig config = Substitute.For<IConfig>();
            config.Url.Returns("http://www.somesite.com");
            Query query = new Query(queryStringBuilder, config);

            Dictionary<string, int> dict = new Dictionary<string, int>()
            {
                {"from", 1},
                {"to", 100}
            };

            // Act
            query
                .Where("id", 123)
                .Where("name", "danny")
                .Where("price", dict);

            // Assert
            Dictionary<string, object> shouldPass = new Dictionary<string, object>()
            {
                {"id", 123},
                {"name", "danny"},
                {"price", dict}
            };
            CollectionAssert.AreEqual(shouldPass, query.WhereMap);
        }

        [TestMethod]
        public void Check_Required_Select()
        {
            // Arrange
            IQueryStringBuilder queryStringBuilder = Substitute.For<IQueryStringBuilder>();
            IConfig config = Substitute.For<IConfig>();
            config.Url.Returns("http://www.somesite.com");
            Query query = new Query(queryStringBuilder, config);

            // Act
            query
                .Select("something");

            // Assert
            Assert.ThrowsException<ArgumentException>(() => query.ToString());
        }

        [TestMethod]
        public void Check_Required_Name()
        {
            // Arrange
            IQueryStringBuilder queryStringBuilder = Substitute.For<IQueryStringBuilder>();
            IConfig config = Substitute.For<IConfig>();
            config.Url.Returns("http://www.somesite.com");
            Query query = new Query(queryStringBuilder, config);

            // Act
            query
                .Name("something");

            // Assert
            Assert.ThrowsException<ArgumentException>(() => query.ToString());
        }

        [TestMethod]
        public void Check_RawNotRequired_NameSelect()
        {
            // Arrange
            IQueryStringBuilder queryStringBuilder = Substitute.For<IQueryStringBuilder>();
            IConfig config = Substitute.For<IConfig>();
            config.Url.Returns("http://www.somesite.com");
            Query query = new Query(queryStringBuilder, config);

            const string rawStr = "something(a:123){id}";

            // Act
            query
                .Raw(rawStr);

            // Assert
            Assert.AreEqual(rawStr, query.ToString());
        }

        [TestMethod] public void Check_Clear()
        {
            // Arrange
            IConfig config = Substitute.For<IConfig>();
            config.Url.Returns("http://www.somesite.com");

            IQueryStringBuilder queryStringBuilder = Substitute.For<IQueryStringBuilder>();
            Query query = new Query(queryStringBuilder, config);

            IQueryStringBuilder batchQueryStringBuilder = Substitute.For<IQueryStringBuilder>();
            Query batchQuery = new Query(batchQueryStringBuilder, config);

            const string expectedSelect = "field";
            const string expectedFrom = "haystack";
            const string expectedAlias = "calhoon";
            const string expectedComment = "this is a comment";
            const string expectedBatchQuery = "someEndpoint(id:1){name}";

            Dictionary<string, object> expectedWhere = new Dictionary<string, object>()
            {
                {"dog", "cat"},
                {"limit", 3}
            };

            // Act
            batchQuery.Raw(expectedBatchQuery);
            query
                .Name(expectedFrom)
                .Select(expectedSelect)
                .Alias(expectedAlias)
                .Where(expectedWhere)
                .Comment(expectedComment)
                .Batch(batchQuery);
                
            // Assert to validate stuff has been set first!
            Assert.AreEqual(expectedFrom, query.QueryName);
            Assert.AreEqual(expectedAlias, query.AliasName);
            CollectionAssert.AreEqual(expectedWhere, query.WhereMap);
            Assert.AreEqual(expectedSelect, query.SelectList.First());
            Assert.AreEqual(1, query.BatchQueryList.Count);

            // Re-act again to clear, this is the actual test...
            query.Clear();

            string emptyStr = string.Empty;
            expectedWhere.Clear();

            // Assert it's all empty
            Assert.AreEqual(emptyStr, query.QueryName);
            Assert.AreEqual(emptyStr, query.AliasName);
            CollectionAssert.AreEqual(expectedWhere, query.WhereMap);
            Assert.AreEqual(0, query.SelectList.Count());
            Assert.AreEqual(emptyStr, query.QueryComment);
            Assert.AreEqual(0, query.BatchQueryList.Count);
        }

        [TestMethod]
        public void Check_Raw_Build()
        {
            // Arrange
            IQueryStringBuilder queryStringBuilder = Substitute.For<IQueryStringBuilder>();
            IConfig config = Substitute.For<IConfig>();
            config.Url.Returns("http://www.somesite.com");
            Query query = new Query(queryStringBuilder, config);

            const string expectedRawQuery = "query{Version}";

            // Act
            query
                .Name("Should Not Exists")
                .Raw(expectedRawQuery);

            // Assert it's exists and returned on build

            Assert.AreEqual(expectedRawQuery, query.RawQuery);
            Assert.AreEqual(expectedRawQuery, query.ToString());
            Assert.AreEqual(string.Empty, query.QueryName);
        }

        [TestMethod]
        public void Check_Error_Exists()
        {
            IQueryStringBuilder queryStringBuilder = Substitute.For<IQueryStringBuilder>();
            IConfig config = Substitute.For<IConfig>();
            config.Url.Returns("http://www.somesite.com");
            Query query = new Query(queryStringBuilder, config);

            // add an empty error to mess with things
            query.GqlErrors.Add(new GraphQLError());

            // Query query = new Query(queryStringBuilder, config);
            
            Assert.IsTrue(query.HasErrors());
        }
    }
}