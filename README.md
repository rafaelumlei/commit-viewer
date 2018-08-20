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
