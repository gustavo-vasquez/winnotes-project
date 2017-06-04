using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProbandoTodo.Models
{
    public class CustomDataAnnotations
    {
        public class FileSizeAttribute : ValidationAttribute
        {
            private readonly int _maxSize;

            public FileSizeAttribute(int maxSize)
            {
                _maxSize = maxSize;
            }

            public override bool IsValid(object value)
            {
                if (value == null) return true;

                return (value as HttpPostedFileBase).ContentLength <= _maxSize;
            }

            public override string FormatErrorMessage(string name)
            {
                return string.Format("*Tamaño máximo de imágen = 2 MB");
            }
        }

        public class FileTypesAttribute : ValidationAttribute
        {
            private readonly List<string> _types;

            public FileTypesAttribute(string types)
            {
                _types = types.Split(',').ToList();
            }

            public override bool IsValid(object value)
            {
                if (value == null) return true;

                var fileExt = System.IO.Path.GetExtension((value as HttpPostedFileBase).FileName).Substring(1);
                return _types.Contains(fileExt, StringComparer.OrdinalIgnoreCase);
            }

            public override string FormatErrorMessage(string name)
            {
                return string.Format("*Foto inválida. Tipos de imágen soportadas: {0}.", String.Join(", ", _types));
            }
        }
    }
}