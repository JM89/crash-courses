# Lesson 3: Storing data in a relational database and Data Access Layer

In the previous lesson, we created a BlogPost API controller with read, create, update and remove blog post of a in-memory list, wiped when the service is stopped. Our next step is to persist the blog post into a database. 

When choosing a data storage for an API, we often choose among the two major data models: RDBMS (Relational Database Management System) or NoSQL. For this lesson, we will use a relational DB as they bring interesting concepts to learn (eg. ORM) and are very much in use. That said, we will write the code so the Data Access Layer (DAL) can be modified later. 

To simulate a database server, we will use a docker image of SQL Server. The base is extended to include the initial SQL scripts. We use a `docker-compose.yml` just to simplify the build the image and run the container. Docker, as well as SQL Server, can be the topics of separated courses so we won't go into much details here. 

**Step 1**: Start your local DB server

Go to the ./Prep folder and in a console, run the following command: 
```
docker-compose up
```

As part of the initial scripts, we create a database called `CrashCourseDB`, then we create a table BlogPost that looks like this:
```sql
CREATE TABLE BlogPost (
    BlogId INT NOT NULL IDENTITY(1,1),
    Title VARCHAR(255) NOT NULL,
    Content NVARCHAR(4000) NOT NULL,
    CreationDate DATETIME NOT NULL
    PRIMARY KEY (BlogId)
);
```

We also insert the two rows:
```sql
INSERT INTO dbo.BlogPost 
VALUES 
('Best practices for writing C# code', 'Blah', GETUTCDATE()),
('How to design a distributed system properly', 'Blah', GETUTCDATE())
```

If you have SQL Server Management Studio (SSMS) installed, go to: 
* Server: `localhost,1433`
* Username: `sa`
* Password: `VerySecret1234`

The results should look like:
![01](./images/01.png)

Our database is now ready. 

**Step 2**: Preparing the DAL 

So far, the code written in the `BlogPostController.cs` class was minimal. Writing code for connecting to a database and transforming the data will require a bit more code. I also mentioned that the code should be easily replaced in case we change our mind and go to a NoSQL solution for storing our data. As a best coding practice, we will almost systematically split the DAL code into separate classes. 

As a result, we will fly over some concepts that will be detailed into Lesson 4, such as Dependency Injection (DI).

in "Models", copy the BlogPostResponse class and rename file and class `BlogPost`. 

Next, create a folder `DataStores` in your project.

Create a new item in the `DataStores`, select Interface and call it `IBlogPostDataStore`.

![02](./images/02.png)

```csharp
namespace CrashCourseApi.Web.DataStores
{
    public interface IBlogPostDataStore
    {
    }
}
```

An interface defines a contract that a class must implement. Interfaces are particularly useful when it comes to DI.

Create a new item in the `DataStores`, select Class and call it `BlogPostDataStore`. Then implements the interface `IBlogPostDataStore` in the `BlogPostDataStore` class.

```csharp
namespace CrashCourseApi.Web.DataStores
{
    public class BlogPostDataStore: IBlogPostDataStore
    {
    }
}
```

In the `BlogPostController.cs`, add:
* a readonly variable of type IBlogPostDataStore
* add a constructor 
* add a `IBlogPostDataStore` parameter 

```csharp
private readonly IBlogPostDataStore _blogPostDataStore;

public BlogPostController(IBlogPostDataStore blogPostDataStore)
{
    _blogPostDataStore = blogPostDataStore;
}
```

In Startup.cs class, add the following code after `services.AddControllers();`:

```csharp
services.AddSingleton<IBlogPostDataStore, BlogPostDataStore>();
```

This will tell your API Controller constructor to **inject** an instance of the class `BlogPostDataStore` (which is unique here, a singleton), whenever it meets the interface `IBlogPostDataStore`.

**Step 4**: Define the contract

Back in the interface, define the following contracts:

```csharp
using CrashCourseApi.Web.Models;
using System.Collections.Generic;

namespace CrashCourseApi.Web.DataStores
{
    public interface IBlogPostDataStore
    {
        IEnumerable<BlogPost> SelectAll();
        BlogPost SelectById(int id);
        void Insert(BlogPost blogPost);
        void Update(BlogPost blogPost);
        void Delete(int id);
    }
}
```

Similarly to the API controller, we have one method per operations. The differences is on the name chosen, I am chosing a more database-friendly terminology.

If we go back to the class now, you will notice that VS isn't specially happy about your class anymore: 

![03](./images/03.png)

If you follow the instructions and implement the interface, some code will be auto-generated for you. 

**Step 4**: Define a simple SQL Connection & Commands

The .NET Core Framework proposes `System.Data.SqlClient` library, that we will use at the beginning to describe few notions. We will then replace it by the [Dapper](https://github.com/StackExchange/Dapper) library in Step 4.

To install the library, right click on the project, Manage Nuget Packages: 

![04](./images/04.png)

Then search for `System.Data.SqlClient` and Install (accept the dependencies):

![05](./images/05.png)

We look at the SelectAll method first: 

```csharp
public IEnumerable<BlogPost> SelectAll()
{
    // Create a list of BlogPost
    var blogPosts = new List<BlogPost>();

    // define where to connect and how
    var conn = new SqlConnection("Data Source=localhost,1433;Initial Catalog=CrashCourseDB;User ID=sa;Password=VerySecret1234!");
    
    // open a db connection
    conn.Open();

    // Query the database
    var command = new SqlCommand("Select BlogId, Title, Content, CreationDate from [BlogPost]", conn);

    using (var reader = command.ExecuteReader())
    {
        // for each row read from the SQL SELECT,
        // we create a new BlogPost object
        // and add it to the list of BlogPost
        while (reader.Read())
        {
            var blogPost = new BlogPost()
            {
                Id = reader.GetInt32(0), // 1st column is BlogId
                Title = reader.GetString(1), // 2nd column is Title 
                Content = reader.GetString(2), // 3rd column is Content 
                CreationDate = reader.GetDateTime(3) // 4th column is CreationDate 
            };
            blogPosts.Add(blogPost);
        }
    }

    // close DB connection
    conn.Close();

    // we return the list of BlogPost made of the database records 
    return blogPosts;
}
```

Now that our method is defined, we still need to call it from the ApiController. 

Change the content of the Get() method: 

```csharp
[HttpGet]
public IEnumerable<BlogPostResponse> Get()
{
    // Call our datastore method
    var blogPostEntities = _blogPostDataStore.SelectAll();

    // Map BlogPost objects to BlogPostResponse objects
    return blogPostEntities.Select(x => new BlogPostResponse() {
        Id = x.Id, 
        Title = x.Title,
        Content = x.Content,
        CreationDate = x.CreationDate
    });
}
```

We can build and run the application to test the access to the database

```
curl -X GET https://localhost:5001/api/blogpost
```

**Step 5**: Code refactoring

Two improvements can be done to this code before we carry on:
* We will move the connection strings to the appSettings (environment specific variable)
* We will use an ORM to simplify the code. 

[Dapper ORM Library](https://github.com/StackExchange/Dapper) is an abstraction of the [System.Data.SqlClient.SqlConnection](https://docs.microsoft.com/en-us/dotnet/api/system.data.sqlclient.sqlconnection?view=dotnet-plat-ext-5.0&viewFallbackFrom=netcore-3.1).

In Oriented Object Programming (OOP), an abstraction handles the complexity of a class or library by hiding unnecessary details from the developer.

TODO