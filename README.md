# CommitViewer

This project implements a console and a REST API to browse git commits for a given github url. 

Two "commit fetchers" were implemented, one gets the commits using the github public API V3 and the other uses the git 
program in the commandline (git log).

A resilience policy with (fallback -> circuit breaker -> timeout) was added, so that when exceptions/timeouts arise while
communicating with the github through a fallback to the commandline implementation will occur.

## Project Structure

* CommitFetcher.Interfaces: all interfaces related with the fetching of git commits operations and its exceptions. 
These interfaces are then materialized by two diffent implementations:
  * CommitFetcher.GitCLI: ICommitFetcher implementation based on the command line;
  * CommitFetcher.Github: ICommitFetcher implementation based on the github REST API v3;

* CommitParser.Interfaces: all interfaces and exception related with parsing git log STDOUT:
  * CommitParser.GitCLI: token (commit, author, etc.) and commmit parsing implementations;

* CommitViewer.API: REST API implementation 
* CommitViewer.Console: console implementation to browse commits;
* CommitViewer.Logger.Interfaces: logger interface to be injected in the whole project to avoid direct references to external code in all projects: 
  * CommitViewer.Logger: log4net wrapper;
* CommitViewer.Model: all models and DTOs shared across projects;


## How to run 

A Visual Studio 2017 is required develop and execute this solution. 

After cloning this project and opening this solution in the VS 2017:

*  RIGHT click the  **CommitViewer.API** project and then click Debug -> Start new instance to execute the **REST API**. 
A browser will open with the help page, open **{HOST}/swagger** to load the swagger WEB UI and test the API.


*  RIGHT click the  **CommitViewer.Console** project and then click Debug -> Start new instance to execute the **Console**. 

## Examples of usage

### REST API

```

curl -X GET --header 'Accept: application/json' '{HOST}/api/commits?url=https%3A%2F%2Fgithub.com%2Frafaelumlei%2Ftsoa.git&page=1&per_page=10'

curl -X GET --header 'Accept: application/json' '{HOST}/api/repositories/rafaelumlei/tsoa/commits?page=1&per_page=10'

```

![SwaggerAPIs](https://github.com/rafaelumlei/commit-viewer/blob/master/assets/swagger.png)

![SwaggerSample](https://github.com/rafaelumlei/commit-viewer/blob/master/assets/swagger_sample.png)

### Console 
```

.\CommitViewer.Console.exe --url https://github.com/angular/material2.git --page 2 --per-page 100

```

![ConsoleSample](https://github.com/rafaelumlei/commit-viewer/blob/master/assets/console_sample.PNG)

