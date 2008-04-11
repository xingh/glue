using System;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using Glue.Data.Mapping;

namespace Glue.Data
{
    /// <summary>
    /// Implementation of the ActiveRecord design pattern to wrap a database table or view into a class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ActiveRecord : IActiveRecord
    {
        #region Provider
        
        /// <summary>
        /// Return the MappingProvider for this class
        /// </summary>
        /// <remarks>
        /// The MappingProvider can to be declared in the Table-attribute.
        
        /// If the MappingProvider is not declared, the default MappingProvider is initialized
        /// from the element "dataprovider" (which, obviously, has to be there...).
        /// </remarks>
        /// <example>
        /// [Table(MappingProvider="dataprovider-account")]
        /// public class Account : ActiveRecord
        /// {
        /// [...]
        /// }
        /// </example>
        public IMappingProvider Provider
        {
            get { return MappingProvider.Get(this.GetType()); }
        }

        #endregion 

        #region IActiveRecord Members

        /// <summary>
        /// Insert the current record
        /// </summary>
        public virtual void Insert()
        {
            Provider.Insert(this);
        }

        /// <summary>
        /// Update the current record
        /// </summary>
        public virtual void Update()
        {
            Provider.Update(this);
        }

        /// <summary>
        /// Delete the current record
        /// </summary>
        public virtual void Delete()
        {
            Provider.Delete(this);
        }

        #endregion
    }
}
