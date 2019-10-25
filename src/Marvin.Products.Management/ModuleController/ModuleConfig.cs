﻿
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Marvin.AbstractionLayer;
using Marvin.Configuration;
using Marvin.Products.Management.Importers;
using Marvin.Serialization;
using Marvin.Tools.Wcf;

namespace Marvin.Products.Management
{
    /// <summary>
    /// Configuration of this module
    /// </summary>
    [DataContract]
    public class ModuleConfig : ConfigBase
    {
        /// <summary>
        /// Constructor which will set default values to the interaction host.
        /// </summary>
        public ModuleConfig()
        {
            InteractionHost = new HostConfig
            {
                BindingType = ServiceBindingType.BasicHttp,
                Endpoint = "ProductInteraction",
                MetadataEnabled = true,
                HelpEnabled = true
            };
        }

        protected override void Initialize()
        {
            base.Initialize();

            // Default importer is always included -> hence the name DEFAULT
            Importers = new List<ProductImporterConfig>
            {
                new DefaultImporterConfig()
            };
        }

        /// <summary>
        /// The interaction host.
        /// </summary>
        [DataMember]
        public HostConfig InteractionHost { get; set; }

        /// <summary>
        /// List of configured importers
        /// </summary>
        [DataMember, Description("List of configured importes")]
        [PluginConfigs(typeof(IProductImporter), false)]
        public List<ProductImporterConfig> Importers { get; set; }

        /// <summary>
        /// Configured strategies for the different product types
        /// </summary>
        [DataMember, Description("Configured strategies for the different product types")]
        [PluginConfigs(typeof(IProductTypeStrategy))]
        public List<ProductTypeConfiguration> TypeStrategies { get; set; }

        /// <summary>
        /// Configured strategies for the different product instances
        /// </summary>
        [DataMember, Description("Configured strategies for the different product instances")]
        [PluginConfigs(typeof(IProductInstanceStrategy))]
        public List<ProductInstanceConfiguration> InstanceStrategies { get; set; }

        /// <summary>
        /// Configured strategies for the different product part links
        /// </summary>
        [DataMember, Description("Configured strategies for the different product part links")]
        [PluginConfigs(typeof(IProductLinkStrategy))]
        public List<ProductLinkConfiguration> LinkStrategies { get; set; }

        /// <summary>
        /// Configured strategies for the different product recipes
        /// </summary>
        [DataMember, Description("Configured strategies for the different product recipes")]
        [PluginConfigs(typeof(IProductRecipeStrategy))]
        public List<ProductRecipeConfiguration> RecipeStrategies { get; set; }
    }
}
