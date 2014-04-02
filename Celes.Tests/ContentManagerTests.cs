using Celes.BusinessLogic;
using Moq;
using Ploeh.AutoFixture;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Celes.Tests
{
	public class ContentManagerTests
	{
		private readonly Fixture _fixture = new Fixture();
		private readonly TestContext _testContext = new TestContext();
		private readonly ContentManager<TestContext, RootContent> _contentManager;

		public ContentManagerTests()
		{
			_fixture.Register<ICollection<ChildContent>>(() => _fixture.CreateMany<ChildContent>().ToList());

			_testContext.Roots.Add(_fixture.CreateAnonymous<RootContent>());

			_contentManager = new ContentManager<TestContext, RootContent>();
		}

		[Fact]
		public void GetRootsByPath()
		{
			var contentInfo = _contentManager.GetContentByPath(_testContext, ContentPath.Root);
			Assert.Equal(_testContext.Roots[0], contentInfo.Content);
		}

		[Fact]
		public void GetChildrenByPath()
		{
			foreach (var root in _testContext.Roots)
			{
				foreach (var child in root.Children)
				{
					var contentInfo = _contentManager.GetContentByPath(_testContext, ContentPath.Parse(child.Alias));
					Assert.Equal(child, contentInfo.Content);
				}
			}
		}
	}
}
