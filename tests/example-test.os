﻿&Анно
Перем ПеременнаяСАннотацией Экспорт;
&Анно(1)
Перем ПеременнаяСАннотациейИПараметром Экспорт;
Перем юТест;
Перем ЭтотОбъек Экспорт;
Перем ЭкспортнаяПеременная Экспорт;
Перем Яшма1 Экспорт;
&Анно(Парам1 = 1)
Перем Яшма2;

Функция Версия() Экспорт
	Возврат "0.1";
КонецФункции

Функция ПолучитьСписокТестов(ЮнитТестирование) Экспорт
	
	юТест = ЮнитТестирование;
	
	ВсеТесты = Новый Массив;
	
	ВсеТесты.Добавить("ТестДолжен_ПроверитьВерсию");
	//ВсеТесты.Добавить("НесуществующийМетод");
	// ВсеТесты.Добавить("МетодОшибка");
	
	Возврат ВсеТесты;
КонецФункции

Процедура ТестДолжен_ПроверитьВерсию() Экспорт
	юТест.ПроверитьРавенство("0.1", Версия());
КонецПроцедуры

Процедура ПриватнаяПроцедура()
КонецПроцедуры

Функция ПриватнаяФункция()
КонецФункции
// Процедура МетодОшибка() Экспорт
	// юТест.ПроверитьРавенство(1,2);
// КонецПроцедуры

&Анно(1)
Функция Яшма2() Экспорт
	Возврат Яшма2;
КонецФункции

&Анно
Процедура МетодСРазнымиПараметрами(
Параметр1, Параметр2 = Неопределено, Параметр3 = Истина, Параметр4 = 1, Параметр5 = "Строка",
Параметр6, Параметр7 = Неопределено, Параметр8 = Истина, Параметр9 = 1, Параметр10 = "Строка") Экспорт
КонецПроцедуры