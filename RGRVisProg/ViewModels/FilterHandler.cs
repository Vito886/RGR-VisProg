// FilterHandler
// Звено, реализующее фильтрацию результирующего массива записей

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using ReactiveUI;
using System.Reactive;
using RGRVisProg.Models;
using Microsoft.Data.Sqlite;
using System.IO;
using System;

namespace RGRVisProg.ViewModels
{
    public class FilterHandler : Handler
    {
        private ObservableCollection<Filter> Filters { get; set; }          // Список фильтров
        private List<Dictionary<string, object?>> ResultTable { get; set; } // Результирующий массив для операций OR
        public FilterHandler(QueryManagerViewModel _QueryManager, string collection)
        {
            QM = _QueryManager;
            ResultTable = new List<Dictionary<string, object?>>();

            // Выбираем список фильтров в зависимости от названия: просто фильтры, иначе - фильтры для группировки
            if (collection == "Filters")
                Filters = QM.Filters;
            else
                Filters = QM.GroupFilters;
        }

        // Обработка условия для AND
        private bool SwitchForChain(Filter filter)
        {
            // Исключение = неуспешный запрос
            try
            {
                switch (filter.Operator)
                {
                    case ">":
                        {
                            QM.ResultTable = QM.ResultTable.Where(item =>
                            double.Parse(item[filter.Column].ToString()) > double.Parse(filter.FilterVal)
                            ).ToList();
                            return true;
                        }
                    case "<":
                        {
                            QM.ResultTable = QM.ResultTable.Where(item =>
                            double.Parse(item[filter.Column].ToString()) < double.Parse(filter.FilterVal)
                            ).ToList();
                            return true;
                        }
                    case "=":
                        {
                            QM.ResultTable = QM.ResultTable.Where(item =>
                            item[filter.Column].ToString() == filter.FilterVal
                            ).ToList();
                            return true;
                        }
                    case ">=":
                        {
                            QM.ResultTable = QM.ResultTable.Where(item =>
                            double.Parse(item[filter.Column].ToString()) >= double.Parse(filter.FilterVal)
                            ).ToList();
                            return true;
                        }
                    case "<>":
                        {
                            QM.ResultTable = QM.ResultTable.Where(item =>
                            item[filter.Column].ToString() != filter.FilterVal
                            ).ToList();
                            return true;
                        }
                    case "<=":
                        {
                            QM.ResultTable = QM.ResultTable.Where(item =>
                            double.Parse(item[filter.Column].ToString()) <= double.Parse(filter.FilterVal)
                            ).ToList();
                            return true;
                        }
                    case "In Range":
                        {
                            string separator = "..";
                            string[] range = filter.FilterVal.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                            if (range.Count() != 2)
                                return false;

                            QM.ResultTable = QM.ResultTable.Where(item =>
                            {
                                double number = double.Parse(item[filter.Column].ToString());
                                if (number >= double.Parse(range[0]) && number <= double.Parse(range[1]))
                                    return true;
                                else
                                    return false;
                            }
                            ).ToList();
                            return true;
                        }
                    case "Not In Range":
                        {
                            string separator = "..";
                            string[] range = filter.FilterVal.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                            if (range.Count() != 2)
                                return false;

                            QM.ResultTable = QM.ResultTable.Where(item =>
                            {
                                double number = double.Parse(item[filter.Column].ToString());
                                if (number < double.Parse(range[0]) || number > double.Parse(range[1]))
                                    return true;
                                else
                                    return false;
                            }
                            ).ToList();
                            return true;
                        }
                    case "Contains":
                        {
                            QM.ResultTable = QM.ResultTable.Where(item =>
                            {
                                string value = item[filter.Column].ToString().ToUpper().Replace(" ", ""); ;
                                string filterVal = filter.FilterVal.ToUpper().Replace(" ", "");
                                if (value.IndexOf(filterVal) != -1 && value.IndexOf(filterVal) != 0)
                                    return true;
                                else
                                    return false;
                            }
                            ).ToList();
                            return true;
                        }
                    case "Not Contains":
                        {
                            QM.ResultTable = QM.ResultTable.Where(item =>
                            {
                                string value = item[filter.Column].ToString().ToUpper().Replace(" ", ""); ;
                                string filterVal = filter.FilterVal.ToUpper().Replace(" ", "");
                                if (value.IndexOf(filterVal) == -1 || value.IndexOf(filterVal) == 0)
                                    return true;
                                else
                                    return false;
                            }
                            ).ToList();
                            return true;
                        }
                    case "Is Null":
                        {
                            QM.ResultTable = QM.ResultTable.Where(item =>
                            {
                                string value = item[filter.Column].ToString();
                                if (value == "None" || value == "0" || value == "0000-00-00 00:00:00"
                                   || value == "00:00" || value.Replace(" ", "") == "" || value.ToUpper() == "NULL"
                                  )
                                    return true;
                                else
                                    return false;
                            }
                            ).ToList();
                            return true;
                        }
                    case "Not Null":
                        {
                            QM.ResultTable = QM.ResultTable.Where(item =>
                            {
                                string value = item[filter.Column].ToString();
                                if (value != "None" && value != "0" && value != "0000-00-00 00:00:00"
                                   && value != "00:00" && value.Replace(" ", "") != "" && value.ToUpper() != "NULL"
                                  )
                                    return true;
                                else
                                    return false;
                            }
                            ).ToList();
                            return true;
                        }
                    case "Belong":
                        {
                            string separator = ",";
                            string[] set = filter.FilterVal.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                            if (set.Count() == 0)
                                return false;

                            for (int i = 0; i < set.Count(); i++)
                            {
                                set[i] = set[i].ToUpper().Replace(" ", "");
                            }

                            QM.ResultTable = QM.ResultTable.Where(item =>
                            {
                                if (set.Contains(item[filter.Column].ToString().ToUpper()))
                                    return true;
                                else
                                    return false;
                            }
                            ).ToList();
                            return true;
                        }
                    case "Not Belong":
                        {
                            string separator = ",";
                            string[] set = filter.FilterVal.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                            if (set.Count() == 0)
                                return false;

                            for (int i = 0; i < set.Count(); i++)
                            {
                                set[i] = set[i].ToUpper().Replace(" ", "");
                            }

                            QM.ResultTable = QM.ResultTable.Where(item =>
                            {
                                if (!set.Contains(item[filter.Column].ToString().ToUpper()))
                                    return true;
                                else
                                    return false;
                            }
                            ).ToList();
                            return true;
                        }
                    default:
                        return false;
                }
            }
            catch
            {
                return false;
            }
        }

        // Обработка условия для OR
        private bool SwitchForUnion(Filter filter)
        {
            // Исключение = неуспешный запрос
            try
            {
                switch (filter.Operator)
                {
                    case ">":
                        {
                            ResultTable = ResultTable.Union(QM.ResultTable.Where(item =>
                            double.Parse(item[filter.Column].ToString()) > double.Parse(filter.FilterVal)
                            ).ToList()).ToList();
                            return true;
                        }
                    case "<":
                        {
                            ResultTable = ResultTable.Union(QM.ResultTable.Where(item =>
                            double.Parse(item[filter.Column].ToString()) < double.Parse(filter.FilterVal)
                            ).ToList()).ToList();
                            return true;
                        }
                    case "=":
                        {
                            ResultTable = ResultTable.Union(QM.ResultTable.Where(item =>
                            item[filter.Column].ToString() == filter.FilterVal
                            ).ToList()).ToList();
                            return true;
                        }
                    case ">=":
                        {
                            ResultTable = ResultTable.Union(QM.ResultTable.Where(item =>
                            double.Parse(item[filter.Column].ToString()) >= double.Parse(filter.FilterVal)
                            ).ToList()).ToList();
                            return true;
                        }
                    case "<>":
                        {
                            ResultTable = ResultTable.Union(QM.ResultTable.Where(item =>
                            item[filter.Column].ToString() != filter.FilterVal
                            ).ToList()).ToList();
                            return true;
                        }
                    case "<=":
                        {
                            ResultTable = ResultTable.Union(QM.ResultTable.Where(item =>
                            double.Parse(item[filter.Column].ToString()) <= double.Parse(filter.FilterVal)
                            ).ToList()).ToList();
                            return true;
                        }
                    case "In Range":
                        {
                            string separator = "..";
                            string[] range = filter.FilterVal.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                            if (range.Count() != 2)
                                return false;

                            ResultTable = ResultTable.Union(QM.ResultTable.Where(item =>
                            {
                                double number = double.Parse(item[filter.Column].ToString());
                                if (number >= double.Parse(range[0]) && number <= double.Parse(range[1]))
                                    return true;
                                else
                                    return false;
                            }
                            ).ToList()).ToList();
                            return true;
                        }
                    case "Not In Range":
                        {
                            string separator = "..";
                            string[] range = filter.FilterVal.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                            if (range.Count() != 2)
                                return false;

                            ResultTable = ResultTable.Union(QM.ResultTable.Where(item =>
                            {
                                double number = double.Parse(item[filter.Column].ToString());
                                if (number < double.Parse(range[0]) || number > double.Parse(range[1]))
                                    return true;
                                else
                                    return false;
                            }
                            ).ToList()).ToList();
                            return true;
                        }
                    case "Contains":
                        {
                            ResultTable = ResultTable.Union(QM.ResultTable.Where(item =>
                            {
                                string value = item[filter.Column].ToString().ToUpper().Replace(" ", ""); ;
                                string filterVal = filter.FilterVal.ToUpper().Replace(" ", "");
                                if (value.IndexOf(filterVal) != -1 && value.IndexOf(filterVal) != 0)
                                    return true;
                                else
                                    return false;
                            }
                            ).ToList()).ToList();
                            return true;
                        }
                    case "Not Contains":
                        {
                            ResultTable = ResultTable.Union(QM.ResultTable.Where(item =>
                            {
                                string value = item[filter.Column].ToString().ToUpper().Replace(" ", ""); ;
                                string filterVal = filter.FilterVal.ToUpper().Replace(" ", "");
                                if (value.IndexOf(filterVal) == -1 || value.IndexOf(filterVal) == 0)
                                    return true;
                                else
                                    return false;
                            }
                            ).ToList()).ToList();
                            return true;
                        }
                    case "Is Null":
                        {
                            ResultTable = ResultTable.Union(QM.ResultTable.Where(item =>
                            {
                                string value = item[filter.Column].ToString();
                                if (value == "None" || value == "0" || value == "0000-00-00 00:00:00"
                                   || value == "00:00" || value.Replace(" ", "") == "" || value.ToUpper() == "NULL"
                                  )
                                    return true;
                                else
                                    return false;
                            }
                            ).ToList()).ToList();
                            return true;
                        }
                    case "Not Null":
                        {
                            ResultTable = ResultTable.Union(QM.ResultTable.Where(item =>
                            {
                                string value = item[filter.Column].ToString();
                                if (value != "None" && value != "0" && value != "0000-00-00 00:00:00"
                                   && value != "00:00" && value.Replace(" ", "") != "" && value.ToUpper() != "NULL"
                                  )
                                    return true;
                                else
                                    return false;
                            }
                            ).ToList()).ToList();
                            return true;
                        }
                    case "Belong":
                        {
                            string separator = ",";
                            string[] set = filter.FilterVal.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                            if (set.Count() == 0)
                                return false;

                            for (int i = 0; i < set.Count(); i++)
                            {
                                set[i] = set[i].ToUpper().Replace(" ", "");
                            }

                            ResultTable = ResultTable.Union(QM.ResultTable.Where(item =>
                            {
                                if (set.Contains(item[filter.Column].ToString().ToUpper()))
                                    return true;
                                else
                                    return false;
                            }
                            ).ToList()).ToList();
                            return true;
                        }
                    case "Not Belong":
                        {
                            string separator = ",";
                            string[] set = filter.FilterVal.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                            if (set.Count() == 0)
                                return false;

                            for (int i = 0; i < set.Count(); i++)
                            {
                                set[i] = set[i].ToUpper().Replace(" ", "");
                            }

                            ResultTable = ResultTable.Union(QM.ResultTable.Where(item =>
                            {
                                if (!set.Contains(item[filter.Column].ToString().ToUpper()))
                                    return true;
                                else
                                    return false;
                            }
                            ).ToList()).ToList();
                            return true;
                        }
                    default:
                        return false;
                }
            }
            catch
            {
                return false;
            }
        }

        // Получаем результат для цепочки с оператором OR, перебирая каждый фильтр и
        // соединяя результаты выборки с результирующим массивом на каждой итерации, который изначально пуст
        private void ResultByUnion()
        {
            foreach (Filter filter in Filters)
            {
                // Для проверки на "пустоту" или "НЕ пустоту" не нужно значение условия
                // Если значение условия пустое - запрос не выполняется
                if (filter.FilterVal.Replace(" ", "") == "" && filter.Operator != "Is Null" && filter.Operator != "Not Null")
                {
                    QM.IsRequestSuccess = false;
                    return;
                }
                else
                {
                    // Вызов обработки условия, если false - запрос не выполняется
                    if (!SwitchForUnion(filter))
                    {
                        QM.IsRequestSuccess = false;
                        return;
                    }
                    else
                    {
                        QM.IsRequestSuccess = true;
                    }
                }
            }
            QM.ResultTable = ResultTable;
        }

        // Получаем результат для цепочки с оператором AND, перебирая каждый фильтр и
        // делая выборку из результирующего массива, занося её результат в него же на каждой итерации
        // Вызывается также если фильтр в списке один
        private void ResultByChain()
        {
            // Если фильтр один и он пустой, то выходим и переходим на следующий обработчик
            if (Filters.Count == 1 && Filters[0].FilterVal == "" && Filters[0].Operator == "" && Filters[0].Column == "")
            {
                QM.IsRequestSuccess = true;
                return;
            }

            foreach (Filter filter in Filters)
            {
                // Для проверки на "пустоту" или "НЕ пустоту" не нужно значение условия
                // Если значение условия пустое - запрос не выполняется
                if (filter.FilterVal.Replace(" ", "") == "" && filter.Operator != "Is Null" && filter.Operator != "Not Null")
                {
                    QM.IsRequestSuccess = false;
                    return;
                }
                else
                {
                    // Вызов обработки условия, если false - запрос не выполняется
                    if (!SwitchForChain(filter))
                    {
                        QM.IsRequestSuccess = false;
                        return;
                    }
                    else
                    {
                        QM.IsRequestSuccess = true;
                    }
                }
            }
        }

        public override void Try()
        {
            // Если окно менеджера запросов не пустое
            if (QM != null)
            {
                // И список фильтров не пуст
                if (Filters.Count != 0)
                {
                    // Проверяем, одно условие или нет и вызываем обработку цепочки, если условие не одно
                    if (Filters.Count > 1)
                    {
                        if (Filters[1].BoolOper == "AND")
                            ResultByChain();
                        else
                            ResultByUnion();
                    }
                    else
                    {
                        ResultByChain();
                    }

                    // Если следующий обработчик существует переходим к нему, при условии успешной фильтрации
                    if (NextHope != null && QM.IsRequestSuccess != false)
                    {
                        NextHope.Try();
                    }
                }
                // Иначе переходим на следующий обработчик, если он есть и выбраны колонки
                else if (NextHope != null && QM.SelectedColumns.Count != 0)
                {
                    NextHope.Try();
                }
                // Иначе - фильтрация не нужна, выходим
                else
                    QM.IsRequestSuccess = true;
            }
            else
            {
                return;
            }
        }
    }
}
