using System;
using System.Collections.Generic;
using System.Linq;
using System.Dynamic;
using Tenaris.Confab.SGM.Domain.Entities;
using Tenaris.Confab.SGM.Repositories;

namespace Tenaris.Confab.SGM.Services
{
    public class OilManagementReportService : IOilManagementReportService
    {
        IOilManagementReportRepository _repo;
        public OilManagementReportService(IOilManagementReportRepository repo)
        {
            _repo = repo;
        }

        public List<ReportLayout> GetReportLayout(int idReport)
        {
            var columnsLayout = new List<ReportLayout>();
            foreach (var row in _repo.GetReportLayout(idReport))
            {
                var colLayout = columnsLayout.Where(x => x.ColumnOrder == ((dynamic)row).ColumnOrder).FirstOrDefault();
                if (colLayout != null)
                {
                    switch ((string)((dynamic)row).attribute)
                    {
                        case "min_width":
                            colLayout.css.min_width = ((dynamic)row).Value;
                            break;
                        case "font_size":
                            colLayout.css.font_size = ((dynamic)row).Value;
                            break;
                        case "min_height":
                            colLayout.css.min_height = ((dynamic)row).Value;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    colLayout = new ReportLayout()
                    {
                        idReport = ((dynamic)row).idReport,
                        Title = ((dynamic)row).Title,
                        ColumnOrder = ((dynamic)row).ColumnOrder,
                        id = ((dynamic)row).id,
                        name = ((dynamic)row).name,
                        haveFilterFunctionality = ((dynamic)row).haveFilterFunctionality,
                        css = new LayoutContainer()
                        {
                            min_width = ((dynamic)row).Value
                        }
                    };

                    columnsLayout.Add(colLayout);
                }
            }

            return columnsLayout;
        }

        private dynamic AddAttributeToContainer(dynamic currentContainer, string newAttributeName, string newAttributeValue)
        {
            dynamic newContainer = null;
            switch (newAttributeName)
            {
                case "min_width":
                    if (!HaveAttribute(currentContainer, newAttributeName) || string.IsNullOrEmpty(((dynamic)currentContainer).min_width))
                    {
                        newContainer = new
                        {
                            min_width = newAttributeValue,
                            font_size = HaveAttribute(currentContainer, "font_size") ? ((dynamic)currentContainer).font_size : null,
                            min_height = HaveAttribute(currentContainer, "min_height") ? ((dynamic)currentContainer).min_height : null,
                        };
                    }
                    break;

                case "font_size":
                    if (!HaveAttribute(currentContainer, newAttributeName) || string.IsNullOrEmpty(((dynamic)currentContainer).font_size))
                    {
                        newContainer = new
                        {
                            min_width = HaveAttribute(currentContainer, "min_width") ? ((dynamic)currentContainer).min_width : null,
                            font_size = newAttributeValue,
                            min_height = HaveAttribute(currentContainer, "min_height") ? ((dynamic)currentContainer).min_height : null,
                        };
                    }
                    break;

                case "min_height":
                    if (!HaveAttribute(currentContainer, newAttributeName) || string.IsNullOrEmpty(((dynamic)currentContainer).min_height))
                    {
                        newContainer = new
                        {
                            min_width = HaveAttribute(currentContainer, "min_width") ? ((dynamic)currentContainer).min_width : null,
                            font_size = HaveAttribute(currentContainer, "font_size") ? ((dynamic)currentContainer).font_size : null,
                            min_height = newAttributeValue
                        };
                    }
                    break;
                default:
                    break;
            }

            return newContainer;
        }

        public static bool HaveAttribute(dynamic obj, string attributeName)
        {
            if (obj is ExpandoObject)
                return ((IDictionary<string, object>)obj).ContainsKey(attributeName);

            return obj.GetType().GetProperty(attributeName) != null;
        }

    }
}
