﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tenon.Repository.EfCore.MySql.Extensions;
using Tenon.Repository.EfCore.MySqlTests.Entities;

namespace Tenon.Repository.EfCore.MySqlTests;

[TestClass]
public class EfRepositoryTests
{
    private readonly IServiceProvider _serviceProvider;

    public EfRepositoryTests()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false)
            .Build();

        _serviceProvider = new ServiceCollection()
            .AddLogging(loggingBuilder => loggingBuilder
                .AddConsole()
                .SetMinimumLevel(LogLevel.Debug))
            .AddEfCoreMySql<MySqlTestDbContext>(configuration.GetSection("MySql"))
            .BuildServiceProvider();
    }

    [TestInitialize]
    public async Task Init()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            using (var context = scope.ServiceProvider.GetService<MySqlTestDbContext>())
            {
                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();
                var blog1 = new Blog { Url = "http://sample.com", Id = 1 };
                var post1 = new Post { Blog = blog1, Content = "test", Title = "test" };
                var blogRepository = new EfRepository<Blog>(context);
                var result = await blogRepository.InsertAsync(blog1);
                Assert.AreEqual(result > 0, true);
                var postRepository = new EfRepository<Post>(context);
                result = await postRepository.InsertAsync(post1);
                Assert.AreEqual(result > 0, true);

                var blog2 = new Blog { Url = "http://sample2.com", Id = 2 };
                var post2 = new Post { Blog = blog2, Content = "test2", Title = "test2" };
                result = await blogRepository.InsertAsync(blog2);
                Assert.AreEqual(result > 0, true);
                result = await postRepository.InsertAsync(post2);
                Assert.AreEqual(result > 0, true);

                var blog3 = new Blog { Url = "http://sample4.com", Id = 3 };
                var post3 = new Post { Blog = blog3, Content = "test3", Title = "test3" };
                result = await blogRepository.InsertAsync(blog3);
                Assert.AreEqual(result > 0, true);
                result = await postRepository.InsertAsync(post3);
                Assert.AreEqual(result > 0, true);

                var blog4 = new Blog { Url = "http://sample4.com", Id = 4 };
                var post4 = new Post { Blog = blog4, Content = "test4", Title = "test4" };
                result = await blogRepository.InsertAsync(blog4);
                Assert.AreEqual(result > 0, true);
                result = await postRepository.InsertAsync(post4);
                Assert.AreEqual(result > 0, true);
            }
        }
    }


    [TestMethod]
    public async Task InsertAsyncTest()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            using (var context = scope.ServiceProvider.GetService<MySqlTestDbContext>())
            {
                var blog = new Blog { Url = "http://sample.com", Id = 88 };
                var blogRepository = new EfRepository<Blog>(context);
                var result = await blogRepository.InsertAsync(blog);
                Assert.AreEqual(result > 0, true);
                var findBlog = await blogRepository.GetAsync(4, false);
                findBlog.Url = "http://sample2.com";
                result = await blogRepository.UpdateAsync(findBlog);
                Assert.AreEqual(result > 0, true);
            }
        }
    }

    [TestMethod]
    public async Task InsertRangeAsyncTest()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            using (var context = scope.ServiceProvider.GetService<MySqlTestDbContext>())
            {
                var bogs = new Blog[5]
                {
                    new() { Url = "http://sample.com", Id = 81 },
                    new() { Url = "http://sample.com", Id = 82 },
                    new() { Url = "http://sample.com", Id = 83 },
                    new() { Url = "http://sample.com", Id = 84 },
                    new() { Url = "http://sample.com", Id = 85 }
                };
                var blogRepository = new EfRepository<Blog>(context);
                var result = await blogRepository.InsertAsync(bogs);
                Assert.AreEqual(result == 5, true);
            }
        }
    }

    [TestMethod]
    public async Task UpdateAsyncTest()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            using (var context = scope.ServiceProvider.GetService<MySqlTestDbContext>())
            {
                var blogRepository = new EfRepository<Blog>(context);
                var blog = await blogRepository.GetAsync(1, false);
                blog.Rating = 100;
                var result = await blogRepository.UpdateAsync(blog);
                Assert.AreEqual(result == 1, true);
            }
        }
    }

    [TestMethod]
    public async Task UpdateRangeAsyncTest()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            using (var context = scope.ServiceProvider.GetService<MySqlTestDbContext>())
            {
                var blogRepository = new EfRepository<Blog>(context);
                var blogs = await blogRepository.GetListAsync(x => x.Id == 1 || x.Id == 2, false);
                foreach (var blog in blogs)
                    blog.Rating = 100;
                var result = await blogRepository.UpdateAsync(blogs);
                Assert.AreEqual(result == 2, true);
            }
        }
    }

    [TestMethod]
    public async Task AnyAsyncTest()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            using (var context = scope.ServiceProvider.GetService<MySqlTestDbContext>())
            {
                var blogRepository = new EfRepository<Blog>(context);
                var result = await blogRepository.AnyAsync(x => x.Id == 1 || x.Id == 2);
                Assert.IsTrue(result);
                result = await blogRepository.AnyAsync(x => x.Id == 11 || x.Id == 22);
                Assert.IsFalse(result);
            }
        }
    }

    [TestMethod]
    public async Task CountAsyncTest()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            using (var context = scope.ServiceProvider.GetService<MySqlTestDbContext>())
            {
                var blogRepository = new EfRepository<Blog>(context);
                var result = await blogRepository.CountAsync(x => x.Id == 1 || x.Id == 2);
                Assert.AreEqual(2, result);
            }
        }
    }


    [TestMethod]
    public async Task RemoveAsyncTest()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            using (var context = scope.ServiceProvider.GetService<MySqlTestDbContext>())
            {
                var blogRepository = new EfRepository<Blog>(context);
                var blog = await blogRepository.GetAsync(1, true);
                var result = await blogRepository.RemoveAsync(blog);
                Assert.AreEqual(1, result);
            }
        }
    }

    [TestMethod]
    public async Task RemoveAsyncTest1()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            using (var context = scope.ServiceProvider.GetService<MySqlTestDbContext>())
            {
                var blogRepository = new EfRepository<Blog>(context);
                var result = await blogRepository.RemoveAsync(1);
                Assert.AreEqual(1, result);
                result = await blogRepository.RemoveAsync(111);
                Assert.AreEqual(0, result);
            }
        }
    }

    [TestMethod]
    public async Task RemoveRangeAsyncTest()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            using (var context = scope.ServiceProvider.GetService<MySqlTestDbContext>())
            {
                var blogRepository = new EfRepository<Blog>(context);
                var blogs = await blogRepository.GetListAsync(x => x.Id == 2 || x.Id == 1, default);
                var result = await blogRepository.RemoveAsync(blogs);
                Assert.AreEqual(2, result);
            }
        }
    }


    [TestMethod]
    public async Task UpdateAsyncTest1()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            using (var context = scope.ServiceProvider.GetService<MySqlTestDbContext>())
            {
                var blogRepository = new EfRepository<Blog>(context);
                var blog = await blogRepository.GetAsync(1, false);
                Assert.IsNotNull(blog);
                blog.Rating = 100000;
                blog.Url = "hello";
                var result = await blogRepository.UpdateAsync(blog, [x => x.Rating]);
                Assert.AreEqual(result == 1, true);
                var newblog = await blogRepository.GetAsync(1, false);
                Assert.AreEqual(100000, newblog?.Rating);
                Assert.AreEqual("http://sample.com", newblog?.Url);
            }
        }
    }
}