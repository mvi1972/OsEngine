﻿using OsEngine.OsTrader.Panels;
using System;
using System.Collections.Generic;
using OsEngine.Entity;
using OsEngine.OsTrader.Panels.Tab;
using OsEngine.Charts.CandleChart.Indicators;
using System.Threading;
using Microsoft.SqlServer.Server;

namespace OsEngine.Robots.MoiRoboti
{
    public class Frank : BotPanel
    {
        public decimal market_prise; // рыночная стоимость товара
        public string kvot_name ; // название квотируемой валюты - инструмента
        public string tovar_name; // название базовая валюты - товара
        private MyBlanks blanks = new MyBlanks("Frank", StartProgram.IsOsTrader); // создание экземпляра класса MyBlanks
        private BotTabSimple _tab; // поле хранения вкладки робота 
        public decimal percent_tovara; // поле хранения % товара 
        public decimal start_sum; // поле хранения стартовой суммы депозита 
        public decimal depo; // количество квотируемой в портфеле
        public decimal tovar; // количество товара  в портфеле


        public Frank(string name, StartProgram startProgram) : base(name, startProgram)
        {
            TabCreate(BotTabType.Simple);  // создание простой вкладки
            _tab = TabsSimple[0]; // записываем первую вкладку в поле
            kvot_name = "USDT";  // тут надо указать - инициализировать название квотируемой валюты (деньги)
            tovar_name = "BTC"; // тут надо указать - инициализировать название товара 

            _tab.BestBidAskChangeEvent += _tab_BestBidAskChangeEvent; // событие изменения лучших цен
            _tab.OrderUpdateEvent += _tab_OrderUpdateEvent;
            
        }

        private void _tab_OrderUpdateEvent(Order obj)
        {
            Switching_mode();
        }

        private void _tab_BestBidAskChangeEvent(decimal arg1, decimal arg2) // событие изменения лучших цен
        {
            market_prise = blanks.price;
            // decimal b = blanks.Balans_kvot();

            Console.WriteLine(" стартовая сумма = " + Start_sum() + " $ ");
            Console.WriteLine(" в портфеле потрачено = " + Percent_tovara() + " % ");
        }
        void Switching_mode()
        {
            Percent_tovara();
            if (90 <= percent_tovara) //  режим выхода 
            {

            }
            if ( 70 <= percent_tovara) //  режим фиксации прибыли
            {

            }
            if (50 <= percent_tovara) // режим набора товара и ожидания прибыли
            {

            }
            if (30 <= percent_tovara) // режим набора товара
            {

            }

        }
        public decimal Percent_tovara() // расчет % объема купленного товара в портфеле 
        {
            decimal st = Start_sum();
            decimal kv = Balans_kvot(kvot_name);
            decimal rasxod = st - kv;
            decimal per = rasxod / st * 100;
            percent_tovara = MyBlanks.Okruglenie(per) ;
            return percent_tovara;
        }
        public decimal Start_sum() // расчет начального состояния портфеля
        {
            Balans_kvot(kvot_name);
            Balans_tovara(tovar_name);
            start_sum = MyBlanks.Okruglenie(depo + tovar * market_prise);
            return start_sum;
        }
        public decimal Balans_kvot(string kvot_name)   // запрос квотируемых средств в портфеле - название присваивается в kvot_name
        {
            List<PositionOnBoard> poses = _tab.Portfolio.GetPositionOnBoard();
            decimal vol_kvot = 0;
            decimal vol_kvot_blok = 0;
            for (int i = 0; i < poses.Count; i++)
            {
                if (poses[i].SecurityNameCode == kvot_name)
                {
                    vol_kvot = poses[i].ValueCurrent;
                    vol_kvot_blok = poses[i].ValueBlocked;
                    break;
                }
            }
            if (vol_kvot != 0)
            {
                depo = vol_kvot + vol_kvot_blok;
            }
            return depo;
        }
        public decimal Balans_tovara(string tovar_name)   // запрос торгуемых средств в портфеле (в BTC ) название присваивается в tovar_name
        {
            List<PositionOnBoard> poses = _tab.Portfolio.GetPositionOnBoard();
            decimal vol_instr = 0;
            decimal vol_instr_blok = 0;
            for (int i = 0; i < poses.Count; i++)
            {
                if (poses[i].SecurityNameCode == tovar_name)
                {
                    vol_instr = poses[i].ValueCurrent;  // текущий объем 
                    vol_instr_blok = poses[i].ValueBlocked; // блокированный объем 
                }
            }
            if (vol_instr != 0)
            {
                tovar = vol_instr + vol_instr_blok;
            }
            return tovar;
        }
        public override string GetNameStrategyType()
        {
            return "Frank";
        }
        public override void ShowIndividualSettingsDialog()
        {
            
        }
    }
}