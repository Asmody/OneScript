
Перем юТест;
Перем СчетчикВызовов;
Перем ПараметрыВызова;

Функция ПолучитьСписокТестов(ЮнитТестирование) Экспорт
	
	юТест = ЮнитТестирование;
	
	ВсеТесты = Новый Массив;
	
	ВсеТесты.Добавить("ТестДолжен_ПроверитьПодпискуНаСобытие");
	ВсеТесты.Добавить("ТестДолжен_ПроверитьОтпискуОтСобытия");
	ВсеТесты.Добавить("ТестДолжен_ПроверитьПодпискуНаСобытиеВВидеЛокальнойФункции");
	ВсеТесты.Добавить("ТестДолжен_ПроверитьОтпискуОтСобытияВВидеЛокальнойФункции");
	ВсеТесты.Добавить("ТестДолжен_ПроверитьЧтоПодпискаПоОбъектуВидитТолькоЭкспорт");
	ВсеТесты.Добавить("ТестДолжен_ПроверитьПодпискуНаСобытиеВВидеВыражения");
	ВсеТесты.Добавить("ТестДолжен_ПроверитьПодпискуСОбработчикомВВидеВыражения");
	ВсеТесты.Добавить("ТестДолжен_ПроверитьЧтоУИсточникаМожетБытьНесколькоСобытий");
	
	Возврат ВсеТесты;

КонецФункции

Процедура ПередЗапускомТеста() Экспорт
	СчетчикВызовов = 0;
	ПараметрыВызова = Неопределено;
КонецПроцедуры

Процедура СгенерироватьСобытие(Знач ИмяСобытия, Знач Параметры) Экспорт
	ВызватьСобытие(ИмяСобытия, Параметры);
КонецПроцедуры

Процедура ОбработчикСобытия(Параметр1, Параметр2) Экспорт
	
	СчетчикВызовов = СчетчикВызовов + 1;
	ПараметрыВызова = Новый Структура("Параметр1, Параметр2", Параметр1, Параметр2);

КонецПроцедуры

Процедура ОбработчикСобытияВнутр(Параметр1, Параметр2)
	
	ОбработчикСобытия(Параметр1, Параметр2);

КонецПроцедуры

Процедура ТестДолжен_ПроверитьПодпискуНаСобытие() Экспорт
	
	МассивПараметров = Новый Массив;
	МассивПараметров.Добавить("П1");
	МассивПараметров.Добавить("П2");

	Источник = Новый ТестСобытий;
	Источник.СгенерироватьСобытие("МоеСобытие", МассивПараметров);
	
	юТест.Проверитьравенство(0, СчетчикВызовов);
	ДобавитьОбработчик Источник.МоеСобытие, ЭтотОбъект.ОбработчикСобытия;

	Источник.СгенерироватьСобытие("МоеСобытие", МассивПараметров);
	юТест.Проверитьравенство(1, СчетчикВызовов);
	юТест.ПроверитьРавенство("П1", ПараметрыВызова.Параметр1);
	юТест.ПроверитьРавенство("П2", ПараметрыВызова.Параметр2);

КонецПроцедуры

Процедура ТестДолжен_ПроверитьПодпискуНаСобытиеВВидеВыражения() Экспорт
	
	МассивПараметров = Новый Массив;
	МассивПараметров.Добавить("П1");
	МассивПараметров.Добавить("П2");

	Источник = Новый ТестСобытий;
	Источник.СгенерироватьСобытие("МоеСобытие", МассивПараметров);
	
	юТест.Проверитьравенство(0, СчетчикВызовов);
	ДобавитьОбработчик Источник["Мое"+"Событие"], ЭтотОбъект.ОбработчикСобытия;

	Источник.СгенерироватьСобытие("МоеСобытие", МассивПараметров);
	юТест.Проверитьравенство(1, СчетчикВызовов);
	юТест.ПроверитьРавенство("П1", ПараметрыВызова.Параметр1);
	юТест.ПроверитьРавенство("П2", ПараметрыВызова.Параметр2);

КонецПроцедуры

Процедура ТестДолжен_ПроверитьПодпискуСОбработчикомВВидеВыражения() Экспорт
	
	МассивПараметров = Новый Массив;
	МассивПараметров.Добавить("П1");
	МассивПараметров.Добавить("П2");

	Источник = Новый ТестСобытий;
	Источник.СгенерироватьСобытие("МоеСобытие", МассивПараметров);
	
	юТест.Проверитьравенство(0, СчетчикВызовов);
	ДобавитьОбработчик Источник.МоеСобытие, ЭтотОбъект["Обработчик"+"События"];

	Источник.СгенерироватьСобытие("МоеСобытие", МассивПараметров);
	юТест.Проверитьравенство(1, СчетчикВызовов);
	юТест.ПроверитьРавенство("П1", ПараметрыВызова.Параметр1);
	юТест.ПроверитьРавенство("П2", ПараметрыВызова.Параметр2);

КонецПроцедуры

Процедура ТестДолжен_ПроверитьОтпискуОтСобытия() Экспорт
	
	Источник = Новый ТестСобытий;

	ДобавитьОбработчик Источник.МоеСобытие, ЭтотОбъект.ОбработчикСобытия;

	МассивПараметров = Новый Массив;
	МассивПараметров.Добавить("П1");
	МассивПараметров.Добавить("П2");

	Источник.СгенерироватьСобытие("МоеСобытие", МассивПараметров);
	юТест.ПроверитьРавенство(1, СчетчикВызовов);

	УдалитьОбработчик Источник.МоеСобытие, ЭтотОбъект.ОбработчикСобытия;

	СчетчикВызовов = 0;
	Источник.СгенерироватьСобытие("МоеСобытие", МассивПараметров);
	юТест.ПроверитьРавенство(0, СчетчикВызовов);

КонецПроцедуры

Процедура ТестДолжен_ПроверитьПодпискуНаСобытиеВВидеЛокальнойФункции() Экспорт

    МассивПараметров = Новый Массив;
    МассивПараметров.Добавить("П1");
    МассивПараметров.Добавить("П2");

    Источник = Новый ТестСобытий;
    Источник.СгенерироватьСобытие("МоеСобытие", МассивПараметров);
    
    юТест.Проверитьравенство(0, СчетчикВызовов);
    ДобавитьОбработчик Источник.МоеСобытие, ОбработчикСобытияВнутр;

    Источник.СгенерироватьСобытие("МоеСобытие", МассивПараметров);
    юТест.Проверитьравенство(1, СчетчикВызовов);
    юТест.ПроверитьРавенство("П1", ПараметрыВызова.Параметр1);
    юТест.ПроверитьРавенство("П2", ПараметрыВызова.Параметр2);

КонецПроцедуры

Процедура ТестДолжен_ПроверитьОтпискуОтСобытияВВидеЛокальнойФункции() Экспорт
    Источник = Новый ТестСобытий;
    
    ОбработчикСобытияВнутр = 42; // проверим, что все равно выберет метод, а не переменную
    ДобавитьОбработчик Источник.МоеСобытие, ОбработчикСобытияВнутр;

    МассивПараметров = Новый Массив;
    МассивПараметров.Добавить("П1");
    МассивПараметров.Добавить("П2");

    Источник.СгенерироватьСобытие("МоеСобытие", МассивПараметров);
    юТест.ПроверитьРавенство(1, СчетчикВызовов);

    УдалитьОбработчик Источник.МоеСобытие, ОбработчикСобытияВнутр;

    СчетчикВызовов = 0;
    Источник.СгенерироватьСобытие("МоеСобытие", МассивПараметров);
    юТест.ПроверитьРавенство(0, СчетчикВызовов);
КонецПроцедуры

Процедура ТестДолжен_ПроверитьЧтоПодпискаПоОбъектуВидитТолькоЭкспорт() Экспорт
    
    Источник = Новый ТестСобытий;
    
    Попытка
        ДобавитьОбработчик Источник.МоеСобытие, ЭтотОбъект.ОбработчикСобытияВнутр;
    Исключение
        юТест.ПроверитьБольше(Найти(ОписаниеОшибки(), "Метод объекта не обнаружен"), 0);
        Возврат;
    КонецПопытки;
    
    ВызватьИсключение "Ожидали исключение, но его не было";
    
КонецПроцедуры

Процедура ТестДолжен_ПроверитьЧтоУИсточникаМожетБытьНесколькоСобытий() Экспорт

	Источник = Новый ТестСобытий;

	ДобавитьОбработчик Источник.МоеСобытие, ОбработчикСобытияВнутр;
	ДобавитьОбработчик Источник.МоеСобытиеВторое, ОбработчикСобытияВнутр;

    МассивПараметров = Новый Массив;
    МассивПараметров.Добавить("П1");
    МассивПараметров.Добавить("П2");

    Источник.СгенерироватьСобытие("МоеСобытие", МассивПараметров);
    юТест.ПроверитьРавенство(1, СчетчикВызовов);

    Источник.СгенерироватьСобытие("МоеСобытиеВторое", МассивПараметров);
    юТест.ПроверитьРавенство(2, СчетчикВызовов);

КонецПроцедуры

ПодключитьСценарий(ТекущийСценарий().Источник, "ТестСобытий");