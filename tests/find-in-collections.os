﻿Перем юТест;

Функция ПолучитьСписокТестов(ЮнитТестирование) Экспорт
	юТест = ЮнитТестирование;

	ВсеТесты = Новый Массив;

	ВсеТесты.Добавить("ТестДолжен_ПроверитьПоиск_БулеваСредиЧисел_ВМассиве");
	ВсеТесты.Добавить("ТестДолжен_ПроверитьПоиск_ЧислаСредиБулевых_ВМассиве");
	ВсеТесты.Добавить("ТестДолжен_ПроверитьПоиск_БулеваСредиЧисел_ВФиксированномМассиве");
	ВсеТесты.Добавить("ТестДолжен_ПроверитьПоиск_ЧислаСредиБулевых_ВФиксированномМассиве");
	
	ВсеТесты.Добавить("ТестДолжен_ПроверитьПоиск_БулеваСредиЧисел_ВСпискеЗначений");
	ВсеТесты.Добавить("ТестДолжен_ПроверитьПоиск_ЧислаСредиБулевых_ВСпискеЗначений");
	
	ВсеТесты.Добавить("ТестДолжен_ПроверитьПоиск_БулеваСредиЧисел_ВТаблицеЗначений");
	ВсеТесты.Добавить("ТестДолжен_ПроверитьПоиск_ЧислаСредиБулевых_ВТаблицеЗначений");
	ВсеТесты.Добавить("ТестДолжен_ПроверитьПоиск_БулеваСредиЧисел_ВСтрокахТаблицыЗначений");
	ВсеТесты.Добавить("ТестДолжен_ПроверитьПоиск_ЧислаСредиБулевых_ВСтрокахТаблицыЗначений");
	
	ВсеТесты.Добавить("ТестДолжен_ПроверитьПоиск_БулеваСредиЧисел_ВДеревеЗначений");
	ВсеТесты.Добавить("ТестДолжен_ПроверитьПоиск_ЧислаСредиБулевых_ВДеревеЗначений");
	ВсеТесты.Добавить("ТестДолжен_ПроверитьПоиск_БулеваСредиЧисел_ВСтрокахДереваЗначений");
	ВсеТесты.Добавить("ТестДолжен_ПроверитьПоиск_ЧислаСредиБулевых_ВСтрокахДереваЗначений");

	Возврат ВсеТесты;
КонецФункции

Процедура ТестДолжен_ПроверитьПоиск_БулеваСредиЧисел_ВМассиве() Экспорт
	Массив = Новый Массив;
	Массив.Добавить(0);
	Массив.Добавить(1);
	ЕстьИстина = Массив.Найти(Истина) <> Неопределено;
	ЕстьЛожь = Массив.Найти(Ложь) <> Неопределено;
	юТест.ПроверитьЛожь( ЕстьИстина );
	юТест.ПроверитьЛожь( ЕстьЛожь );
КонецПроцедуры

Процедура ТестДолжен_ПроверитьПоиск_ЧислаСредиБулевых_ВМассиве() Экспорт
	Массив = Новый Массив;
	Массив.Добавить(Ложь);
	Массив.Добавить(Истина);
	Есть1 = Массив.Найти(1) <> Неопределено;
	Есть0 = Массив.Найти(0) <> Неопределено;
	юТест.ПроверитьЛожь( Есть1 );
	юТест.ПроверитьЛожь( Есть0 );
КонецПроцедуры


Процедура ТестДолжен_ПроверитьПоиск_БулеваСредиЧисел_ВФиксированномМассиве() Экспорт
	Массив = Новый Массив;
	Массив.Добавить(0);
	Массив.Добавить(1);
	ФиксированныйМассив = Новый ФиксированныйМассив(Массив);
	ЕстьИстина = ФиксированныйМассив.Найти(Истина) <> Неопределено;
	ЕстьЛожь = ФиксированныйМассив.Найти(Ложь) <> Неопределено;
	юТест.ПроверитьЛожь( ЕстьИстина );
	юТест.ПроверитьЛожь( ЕстьЛожь );
КонецПроцедуры

Процедура ТестДолжен_ПроверитьПоиск_ЧислаСредиБулевых_ВФиксированномМассиве() Экспорт
	Массив = Новый Массив;
	Массив.Добавить(Ложь);
	Массив.Добавить(Истина);
	ФиксированныйМассив = Новый ФиксированныйМассив(Массив);
	Есть1 = ФиксированныйМассив.Найти(1) <> Неопределено;
	Есть0 = ФиксированныйМассив.Найти(0) <> Неопределено;
	юТест.ПроверитьЛожь( Есть1 );
	юТест.ПроверитьЛожь( Есть0 );
КонецПроцедуры


Процедура ТестДолжен_ПроверитьПоиск_БулеваСредиЧисел_ВСпискеЗначений() Экспорт
	СписокЗначений = Новый СписокЗначений();
	СписокЗначений.Добавить(1);
	СписокЗначений.Добавить(0);
	ЕстьИстина = СписокЗначений.НайтиПоЗначению(Истина) <> Неопределено;
	ЕстьЛожь = СписокЗначений.НайтиПоЗначению(Ложь) <> Неопределено;
	юТест.ПроверитьЛожь( ЕстьИстина );
	юТест.ПроверитьЛожь( ЕстьЛожь );
КонецПроцедуры

Процедура ТестДолжен_ПроверитьПоиск_ЧислаСредиБулевых_ВСпискеЗначений() Экспорт
	СписокЗначений = Новый СписокЗначений();
	СписокЗначений.Добавить(Истина);
	СписокЗначений.Добавить(Ложь);
	Есть1 = СписокЗначений.НайтиПоЗначению(1) <> Неопределено;
	Есть0 = СписокЗначений.НайтиПоЗначению(0) <> Неопределено;
	юТест.ПроверитьЛожь( Есть1 );
	юТест.ПроверитьЛожь( Есть0 );
КонецПроцедуры


Процедура ТестДолжен_ПроверитьПоиск_БулеваСредиЧисел_ВТаблицеЗначений() Экспорт
	ТаблицаЗначений = Новый ТаблицаЗначений(); 
	ТаблицаЗначений.Колонки.Добавить("Кол1"); 
	ТаблицаЗначений.Колонки.Добавить("Кол2"); 
	Строка1 = ТаблицаЗначений.Добавить();
	Строка2 = ТаблицаЗначений.Добавить();
	Строка1.Кол1 = 1;
	Строка1.Кол2 = 0;
	Строка2.Кол1 = 0;
	Строка2.Кол2 = 1;
	ЕстьИстина = ТаблицаЗначений.Найти(Истина) <> Неопределено;
	ЕстьЛожь = ТаблицаЗначений.Найти(Ложь) <> Неопределено;
	юТест.ПроверитьЛожь( ЕстьИстина );
	юТест.ПроверитьЛожь( ЕстьЛожь );
КонецПроцедуры

Процедура ТестДолжен_ПроверитьПоиск_ЧислаСредиБулевых_ВТаблицеЗначений() Экспорт
	ТаблицаЗначений = Новый ТаблицаЗначений(); 
	ТаблицаЗначений.Колонки.Добавить("Кол1"); 
	ТаблицаЗначений.Колонки.Добавить("Кол2"); 
	Строка1 = ТаблицаЗначений.Добавить();
	Строка2 = ТаблицаЗначений.Добавить();
	Строка1.Кол1 = Истина;
	Строка1.Кол2 = Ложь;
	Строка2.Кол1 = Ложь;
	Строка2.Кол2 = Истина;
	Есть1 = ТаблицаЗначений.Найти(1) <> Неопределено;
	Есть0 = ТаблицаЗначений.Найти(0) <> Неопределено;
	юТест.ПроверитьЛожь( Есть1 );
	юТест.ПроверитьЛожь( Есть0 );
КонецПроцедуры

Процедура ТестДолжен_ПроверитьПоиск_БулеваСредиЧисел_ВСтрокахТаблицыЗначений() Экспорт
	ТаблицаЗначений = Новый ТаблицаЗначений(); 
	ТаблицаЗначений.Колонки.Добавить("Кол1"); 
	ТаблицаЗначений.Колонки.Добавить("Кол2"); 
	Строка1 = ТаблицаЗначений.Добавить();
	Строка2 = ТаблицаЗначений.Добавить();
	Строка1.Кол1 = 1;
	Строка1.Кол2 = 0;
	Строка2.Кол1 = 0;
	Строка2.Кол2 = 1;
	ЕстьИстина = ТаблицаЗначений.НайтиСтроки( Новый Структура("Кол1", Истина) ).Количество() > 0;
	ЕстьЛожь = ТаблицаЗначений.НайтиСтроки( Новый Структура("Кол1", Ложь) ).Количество() > 0;
	юТест.ПроверитьЛожь( ЕстьИстина );
	юТест.ПроверитьЛожь( ЕстьЛожь );
КонецПроцедуры

Процедура ТестДолжен_ПроверитьПоиск_ЧислаСредиБулевых_ВСтрокахТаблицыЗначений() Экспорт
	ТаблицаЗначений = Новый ТаблицаЗначений(); 
	ТаблицаЗначений.Колонки.Добавить("Кол1"); 
	ТаблицаЗначений.Колонки.Добавить("Кол2"); 
	Строка1 = ТаблицаЗначений.Добавить();
	Строка2 = ТаблицаЗначений.Добавить();
	Строка1.Кол1 = Истина;
	Строка1.Кол2 = Ложь;
	Строка2.Кол1 = Ложь;
	Строка2.Кол2 = Истина;
	Есть1 = ТаблицаЗначений.НайтиСтроки( Новый Структура("Кол1", 1) ).Количество() > 0;
	Есть0 = ТаблицаЗначений.НайтиСтроки( Новый Структура("Кол1", 0) ).Количество() > 0;
	юТест.ПроверитьЛожь( Есть1 );
	юТест.ПроверитьЛожь( Есть0 );
КонецПроцедуры


Процедура ТестДолжен_ПроверитьПоиск_БулеваСредиЧисел_ВДеревеЗначений() Экспорт
	ДеревоЗначений = Новый ДеревоЗначений(); 
	ДеревоЗначений.Колонки.Добавить("Кол1"); 
	ДеревоЗначений.Колонки.Добавить("Кол2"); 
	Строка1 = ДеревоЗначений.Строки.Добавить();
	Строка2 = ДеревоЗначений.Строки.Добавить();
	Строка1.Кол1 = 1;
	Строка1.Кол2 = 0;
	Строка2.Кол1 = 0;
	Строка2.Кол2 = 1;
	ЕстьИстина = ДеревоЗначений.Строки.Найти(Истина) <> Неопределено;
	ЕстьЛожь = ДеревоЗначений.Строки.Найти(Ложь) <> Неопределено;
	юТест.ПроверитьЛожь( ЕстьИстина );
	юТест.ПроверитьЛожь( ЕстьЛожь );
КонецПроцедуры

Процедура ТестДолжен_ПроверитьПоиск_ЧислаСредиБулевых_ВДеревеЗначений() Экспорт
	ДеревоЗначений = Новый ДеревоЗначений(); 
	ДеревоЗначений.Колонки.Добавить("Кол1"); 
	ДеревоЗначений.Колонки.Добавить("Кол2"); 
	Строка1 = ДеревоЗначений.Строки.Добавить();
	Строка2 = ДеревоЗначений.Строки.Добавить();
	Строка1.Кол1 = Истина;
	Строка1.Кол2 = Ложь;
	Строка2.Кол1 = Ложь;
	Строка2.Кол2 = Истина;
	Есть1 = ДеревоЗначений.Строки.Найти(1) <> Неопределено;
	Есть0 = ДеревоЗначений.Строки.Найти(0) <> Неопределено;
	юТест.ПроверитьЛожь( Есть1 );
	юТест.ПроверитьЛожь( Есть0 );
КонецПроцедуры

Процедура ТестДолжен_ПроверитьПоиск_БулеваСредиЧисел_ВСтрокахДереваЗначений() Экспорт
	ДеревоЗначений = Новый ДеревоЗначений(); 
	ДеревоЗначений.Колонки.Добавить("Кол1"); 
	ДеревоЗначений.Колонки.Добавить("Кол2"); 
	Строка1 = ДеревоЗначений.Строки.Добавить();
	Строка2 = ДеревоЗначений.Строки.Добавить();
	Строка1.Кол1 = 1;
	Строка1.Кол2 = 0;
	Строка2.Кол1 = 0;
	Строка2.Кол2 = 1;
	ЕстьИстина = ДеревоЗначений.Строки.НайтиСтроки( Новый Структура("Кол1", Истина) ).Количество() > 0;
	ЕстьЛожь = ДеревоЗначений.Строки.НайтиСтроки( Новый Структура("Кол1", Ложь) ).Количество() > 0;
	юТест.ПроверитьЛожь( ЕстьИстина );
	юТест.ПроверитьЛожь( ЕстьЛожь );
КонецПроцедуры

Процедура ТестДолжен_ПроверитьПоиск_ЧислаСредиБулевых_ВСтрокахДереваЗначений() Экспорт
	ДеревоЗначений = Новый ДеревоЗначений(); 
	ДеревоЗначений.Колонки.Добавить("Кол1"); 
	ДеревоЗначений.Колонки.Добавить("Кол2"); 
	Строка1 = ДеревоЗначений.Строки.Добавить();
	Строка2 = ДеревоЗначений.Строки.Добавить();
	Строка1.Кол1 = Истина;
	Строка1.Кол2 = Ложь;
	Строка2.Кол1 = Ложь;
	Строка2.Кол2 = Истина;
	Есть1 = ДеревоЗначений.Строки.НайтиСтроки( Новый Структура("Кол1", 1) ).Количество() > 0;
	Есть0 = ДеревоЗначений.Строки.НайтиСтроки( Новый Структура("Кол1", 0) ).Количество() > 0;
	юТест.ПроверитьЛожь( Есть1 );
	юТест.ПроверитьЛожь( Есть0 );
КонецПроцедуры
