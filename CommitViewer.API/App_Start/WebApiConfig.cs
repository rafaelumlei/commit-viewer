using CommitFetcher.GitCLI;
using CommitFetcher.Github;
using CommitFetcher.Interfaces;
using CommitParser.GitCLI;
using CommitParser.Interfaces;
using CommitViewer.API.Controllers.WebAPI;
using CommitViewer.API.DependencyResolver;
using CommitViewer.API.Properties;
using CommitViewer.Logger;
using CommitViewer.Logger.Interfaces;
using CommitViewer.Model.DTOs;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using Unity.log4net;

namespace CommitViewer.API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            var container = new UnityContainer();

            // adding ILog (from lo4net) provider
            container.AddNewExtension<Log4NetExtension>();
            container.RegisterType<ICommitViewerLog, Log4NetWrapper>(new ContainerControlledLifetimeManager());

            // adding the two resolve alternatives for ICommitFetcher
            container.RegisterType<ICommitFetcher<CommitDTO>, GitCLICommitFetcher>("console", new ContainerControlledLifetimeManager());
            container.RegisterType<ICommitFetcher<CommitDTO>, GithubCommitFetcher>("http", new ContainerControlledLifetimeManager());

            // adding fetching and parsing related singletons
            container.RegisterType<ICommitParser, CommitParser.GitCLI.CommitParser>(new ContainerControlledLifetimeManager());
            container.RegisterType<ICommitIdParser, CommitIdParser>(new ContainerControlledLifetimeManager());
            container.RegisterType<ICommitAuthorParser, CommitAuthorParser>(new ContainerControlledLifetimeManager());
            container.RegisterType<ICommitDateParser, CommitDateParser>(new ContainerControlledLifetimeManager());
            container.RegisterType<ICommitMessageParser, CommitMessageParser>(new ContainerControlledLifetimeManager());

            // giving a new policy per controller instance
            container.RegisterType<IAsyncResult>(new InjectionFactory((c) =>
            {
                var logger = container.Resolve<ICommitViewerLog>();
                var timeout = Policy.TimeoutAsync(Settings.Default.APIOperationTimeoutSeconds);
                int maxFailures = Settings.Default.MaxSequentialAPIOperationFailures;
                TimeSpan breakingPeriod = TimeSpan.FromSeconds(Settings.Default.APICircuitBreakPeriodSeconds);

                var circuitBreaker = Polly.Policy
                    .Handle<Exception>()
                    .CircuitBreakerAsync(maxFailures,
                        breakingPeriod,
                        onBreak: (e, ts) =>
                        {
                            logger.Error($"Circuit {c.GetType().ToString()}: onBreak", e);
                        },
                        onReset: () =>
                        {
                            logger.Error($"Circuit {c.GetType().ToString()}: onReset");
                        },
                        onHalfOpen: () =>
                        {
                            logger.Error($"Circuit {c.GetType().ToString()}: onHalfOpen");
                        });

                return circuitBreaker.WrapAsync(timeout);
            }));

            container.RegisterType<CommitsController>(new ContainerControlledLifetimeManager(),
                new InjectionConstructor(
                        new ResolvedParameter<ICommitFetcher<CommitDTO>>("http"),
                        new ResolvedParameter<ICommitFetcher<CommitDTO>>("console"),
                        new ResolvedParameter<IAsyncPolicy>(),
                        new ResolvedParameter<ICommitViewerLog>()));

            config.DependencyResolver = new UnityResolver(container);

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
