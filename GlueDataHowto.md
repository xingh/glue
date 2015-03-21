# Glue.Data #

In this tutorial you start with a simple (one-page) asp.net web application. On this page you will then display a list of blog messages that you load from a database, using Glue.Data.

Glue.Data is Glue's data access layer. It helps you to map your .Net classes to your SQL data tables.

#### With Glue.Data you can: ####
  * Map .Net classes to SQL database tables
  * Save, delete, list and search stored objects in a database independent way

#### What Glue.Data does not do: ####
  * Glue.Data does not create or alter the database schema

In later parts of this tutorial, you will use Glue's form processing and Model/View/Controller classes.

# Web application setup #

Setup your asp.net web application with a single Default.aspx page. The directory structure will look like this:

```
    /App_Data
    /bin
        glue.data.dll
        glue.lib.dll
        System.Data.SQLite.dll
    Default.aspx
    Default.aspx.cs
    Web.config
```

Alternatively, you can download the glue source and add the Glue.Lib.csproj, Glue.Data.csproj and Glue.Data.SQLite.csproj projects to your solution.

Add references to the Glue.Data.SQLite and Glue.Data dll's to your project.

# The data model #

Glue supports most SQL variants like MSSQL, MySQL and Oracle. In this howto we will use SQLite because is it easier to install on most systems, but you can use any database for which a DataProvider exists. They are listed on the [DataProvider](http://www.glueproject.com/api/html/T_Glue_Data_BaseDataProvider.htm) page.

Create a 'Message' class to use on your blog. To make it Glue.Data enabled, add the `[Table`] attribute.

```
using System;
using Glue.Data;
using Glue.Data.Mapping;

/// <summary>
/// A Blog message
/// </summary>
[Table]
public class Message
{
    [AutoKey]
    public int Id;

    [Column(MaxLength = 10000)]
    public string Content;

    [Column(MaxLength = 100)]
    public string Author;

    public DateTime Published;

    public Message()
    {
        Published = DateTime.Now;
    }
}
```

Glue uses the attributes in [Glue.Data.Mapping](http://www.glueproject.com/api/html/N_Glue_Data_Mapping.htm) to map your classes. It will map all public members unless they have an `[Exclude`] attribute. If the name of a database column is different than your class member, use the `[Column(Name="SomeOtherName")`] attribute. For all attributes, see the [API](http://www.glueproject.com/api/html/N_Glue_Data_Mapping.htm)

# The web page #

Glue uses Data Providers to access data. In Default.aspx's code-behind page, create a new SQLite data provider.

```
using System;
using System.IO;
using Glue.Data;
using Glue.Data.Providers.SQLite;

namespace GlueBlog
{
    public partial class _Default : System.Web.UI.Page
    {
        public IDataProvider provider;

        protected void Page_Load(object sender, EventArgs e)
        {
            string dbPath = Server.MapPath("App_Data/blog.db3");
            if (!System.IO.File.Exists(dbPath))
            {
                // If the database doesn't exist, create it by adding 'New=True' to 
                // SQLite's connection string.
                provider = new SQLiteDataProvider("Data Source=" + dbPath + ";New=True");
                // Create the table
                provider.ExecuteNonQuery(@"
                        CREATE TABLE [Message] (
                            [id]        INTEGER PRIMARY KEY,
                            [content]   VARCHAR(10000),
                            [author]    VARCHAR(100),
                            [published] DATETIME
                        );
                ");
            }
            else
            {
                provider = new SQLiteDataProvider(null, dbPath, null, null);
            }

            // Initialise the blog with a welcome message.
            if (provider.Count<Message>(null) == 0)
            {
                Message m = new Message();
                m.Author = "Administrator";
                m.Content = "Welcome! This blog has just been created.";
                provider.Insert(m);
            }
        }
    }
}
```
_default.aspx.cs_

Here, we used the DataProvider method 'Count' to count the number of messages. If the blog is empty, the first blog message is created programmatically and inserted in the database with provider.Insert().

In Default.aspx, you can use the provider to get a list of messages to display:

```
<%@ Page Language="C#" CodeBehind="Default.aspx.cs" Inherits="GlueBlog._Default" %>
<html>
<head>
    <title>The Glue Blog</title>
</head>
<body>
    <div>
        <table>
            <tr>
                <th>id</th>
                <th>author</th>
                <th>message</th>
                <th>date</th>
            </tr>
            <% foreach (Message m in provider.List<Message>(null, null, null)) { %>
                <tr>
                    <td><%= m.Id %></td>
                    <td><%= m.Author %></td>
                    <td><%= m.Content %></td>
                    <td><%= m.Published %></td>
                </tr>
            <% } %>
        </table>
    </div>
</body>
</html>
```
_default.aspx_

# Glue configuration #

One more thing you need to do is to add the assemblies containing the Message class and the Glue classes to the glue configuration. Add a "settings" section to Web.config with these assemblies:

```
<configSections>
    <section name="settings" type="Glue.Lib.Configuration,glue.lib" />
</configSections>

<settings>
    <!-- Dynamic compilation options -->
    <compilation>
        <assemblies>
            <add assembly="System.Data.SQLite" />
            <add assembly="Glue.Lib" />
            <add assembly="Glue.Data" />
            <add assembly="GlueBlog" />
        </assemblies>
        <imports>
        </imports>
    </compilation>
</settings>
```
_Web.config (part)_