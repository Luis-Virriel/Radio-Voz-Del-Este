using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ObligatorioProgramacion3_Francisco_Luis.Models.Validations
{
    public class MayorEdad : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null)
                return false;

            DateTime fechaNacimiento;
            if (DateTime.TryParse(value.ToString(), out fechaNacimiento))
            {
                var hoy = DateTime.Today;
                int edad = hoy.Year - fechaNacimiento.Year;

                if (fechaNacimiento > hoy.AddYears(-edad))
                    edad--;

                return edad >= 18;
            }
            return false;
        }

        

    }
}