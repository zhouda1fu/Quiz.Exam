﻿using Microsoft.OpenApi.Models;
using NetCorePal.Extensions.Domain;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Quiz.Exam.Web.Extensions
{
    public static class SwaggerGenOptionsExtionsions
    {
        public static SwaggerGenOptions AddEntityIdSchemaMap(this SwaggerGenOptions swaggerGenOptions)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()
                         .Where(p => p.FullName != null && p.FullName.Contains("Quiz.Exam")))
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsClass && Array.Exists(type.GetInterfaces(), p => p == typeof(IEntityId)))
                    {
                        swaggerGenOptions.MapType(type,
                            () => new OpenApiSchema { Type = typeof(string).Name.ToLower() });
                    }
                }
            }

            return swaggerGenOptions;
        }
    }
}