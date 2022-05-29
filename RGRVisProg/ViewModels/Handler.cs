using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using ReactiveUI;
// Handler
// Абстрактный класс для шаблона "Цепочка обязанностей"

using System.Reactive;
using RGRVisProg.Models;
using Microsoft.Data.Sqlite;
using System.IO;
using System;

namespace RGRVisProg.ViewModels
{
    public abstract class Handler
    {
        protected QueryManagerViewModel? QM { get; set; } = null;   // Окно менеджера запросов
        public Handler? NextHope { get; set; }                      // Следующий обработчик
        public abstract void Try();                                 // Обрабатывающая функция
    }
}
