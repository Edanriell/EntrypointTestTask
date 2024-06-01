using DefaultValueAttribute = System.ComponentModel.DefaultValueAttribute;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

using Server.Attributes.Shared;

namespace Server.DTO.Shared
{
    [SwaggerSchema("GetRequestDTO<T, Filter> with paging, sorting, and custom filtering options.")]
    public class GetRequestDTO<T, Filter> : IValidatableObject
    {
        [DefaultValue(0)]
        [PageIndexValidatorAttribute]
        [SwaggerParameter("Page index (starting from 0).")]
        public int PageIndex { get; set; } = 0;

        [DefaultValue(1)]
        [Range(1, 100)]
        [SwaggerParameter("Page size (number of items per page, range 1-100).")]
        public int PageSize { get; set; } = 1;

        [DefaultValue("Id")]
        [SwaggerParameter("Column to sort by (default is 'Id').")]
        public string? SortColumn { get; set; } = "Id";

        [SortOrderValidator]
        [DefaultValue("ASC")]
        [SwaggerParameter("Sort order ('ASC' or 'DESC', default is 'ASC').")]
        public string? SortOrder { get; set; } = "DESC";

        [DefaultValue(null)]
        [SwaggerParameter("Custom filters.")]
        public Filter? CustomFilters { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new SortColumnValidatorAttribute(typeof(T));
            var result = validator.GetValidationResult(SortColumn, validationContext);
            return (result != null) ? new[] { result } : new ValidationResult[0];
        }
    }
}
