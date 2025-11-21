using System.ComponentModel.DataAnnotations;

namespace ToDoListApi.Validation
{
    /// <summary>
    /// Validates that a string is not null, empty, or whitespace-only after trimming.
    /// </summary>
    public class TrimmedRequiredAttribute : ValidationAttribute
    {
        public TrimmedRequiredAttribute()
        {
            ErrorMessage = "Title is required";
        }

        public override bool IsValid(object? value)
        {
            if (value is not string str)
            {
                return false;
            }

            return !string.IsNullOrWhiteSpace(str.Trim());
        }
    }
}

