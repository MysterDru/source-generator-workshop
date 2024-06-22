// See https://aka.ms/new-console-template for more information

using AutoProperty.Sample;

[assembly: AutoProperty.Generator.AutoProperty(typeof(IAuditMetadata))]
[assembly: AutoProperty.Generator.AutoProperty(typeof(IHasId))]
[assembly: AutoProperty.Generator.AutoProperty(typeof(IHasActiveFlag))]

Console.WriteLine("Hello, World!");