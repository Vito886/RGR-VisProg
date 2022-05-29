// GroupHandler
// Звено, реализующее группировку результирующего массива записей

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
    public class GroupHandler : Handler
    {
        public GroupHandler(QueryManagerViewModel _QueryManager)
        {
            QM = _QueryManager;
        }

        public override void Try()
        {
            // Если окно менеджера запросов не пустое
            if (QM != null)
            {
                // Если группировка по колонке выбрана
                if (QM.GroupingColumn != null)
                {
                    try
                    {
                        var result = QM.ResultTable.GroupBy(item => item[QM.GroupingColumn]).ToList();
                        QM.ResultTable.Clear();
                        foreach (var group in result)
                        {
                            foreach (Dictionary<string, object?> row in group)
                            {
                                QM.ResultTable.Add(row);
                            }
                        }

                        // Если результат не пуст
                        if (QM.ResultTable.Count != 0)
                        {
                            QM.IsRequestSuccess = true;

                            // Переходим на следующий обработчик
                            if (NextHope != null)
                            {
                                NextHope.Try();
                            }
                        }

                        // Иначе ошибка
                        else
                        {
                            QM.IsRequestSuccess = false;
                            return;
                        }
                    }
                    catch
                    {
                        QM.IsRequestSuccess = false;
                        return;
                    }
                }

                // Иначе проверяем, что фильтр группировки пуст и успешно завершаем запрос
                else if (QM.GroupFilters.Count == 1 && QM.GroupFilters[0].FilterVal == ""
                        && QM.GroupFilters[0].Operator == "" && QM.GroupFilters[0].Column == "")
                {
                    QM.IsRequestSuccess = true;
                    return;
                }

                // Иначе ошибка
                else
                {
                    QM.IsRequestSuccess = false;
                    return;
                }
            }
            else
            {
                return;
            }
        }
    }
}
