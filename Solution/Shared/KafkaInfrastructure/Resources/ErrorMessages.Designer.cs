﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KafkaInfrastructure.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class ErrorMessages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ErrorMessages() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("KafkaInfrastructure.Resources.ErrorMessages", typeof(ErrorMessages).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Commit error: {0}.
        /// </summary>
        internal static string CommitError {
            get {
                return ResourceManager.GetString("CommitError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Consume error: {0}.
        /// </summary>
        internal static string ConsumeError {
            get {
                return ResourceManager.GetString("ConsumeError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Delivery failed: {0}.
        /// </summary>
        internal static string DeliveryFailed {
            get {
                return ResourceManager.GetString("DeliveryFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error: {0}.
        /// </summary>
        internal static string GeneralError {
            get {
                return ResourceManager.GetString("GeneralError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid parameter.
        /// </summary>
        internal static string InvalidParameter {
            get {
                return ResourceManager.GetString("InvalidParameter", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Argument topicName is null or empty.
        /// </summary>
        internal static string InvalidTopicName {
            get {
                return ResourceManager.GetString("InvalidTopicName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Partition offset exception: {0}.
        /// </summary>
        internal static string PartitionOffsetError {
            get {
                return ResourceManager.GetString("PartitionOffsetError", resourceCulture);
            }
        }
    }
}
