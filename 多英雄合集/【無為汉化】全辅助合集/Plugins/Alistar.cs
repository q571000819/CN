﻿#region LICENSE

// Copyright 2014-2015 Support
// Alistar.cs is part of Support.
// 
// Support is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Support is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Support. If not, see <http://www.gnu.org/licenses/>.
// 
// Filename: Support/Support/Alistar.cs
// Created:  01/10/2014
// Date:     24/01/2015/13:14
// Author:   h3h3

#endregion

namespace Support.Plugins
{
    #region

    using System;
    using LeagueSharp;
    using LeagueSharp.Common;
    using Support.Util;
    using ActiveGapcloser = Support.Util.ActiveGapcloser;

    #endregion

    public class Alistar : PluginBase
    {
        public Alistar()
        {
            Q = new Spell(SpellSlot.Q, 365);
            W = new Spell(SpellSlot.W, 650);
            E = new Spell(SpellSlot.E, 575);
            R = new Spell(SpellSlot.R, 0);

            W.SetTargetted(0.5f, float.MaxValue);
        }

        public override void OnUpdate(EventArgs args)
        {
            if (ComboMode)
            {
                if (Q.CastCheck(Target, "Combo.Q"))
                {
                    Q.Cast();
                }

                if (Q.IsReady() && W.CastCheck(Target, "Combo.W"))
                {
                    W.CastOnUnit(Target);
                    var jumpTime = Math.Max(0, Player.Distance(Target) - 500) * 10 / 25 + 25;
                    Utility.DelayAction.Add((int) jumpTime, () => Q.Cast());
                }

                var ally = Helpers.AllyBelowHp(ConfigValue<Slider>("Combo.E.Health").Value, E.Range);
                if (E.CastCheck(ally, "Combo.E", true, false))
                {
                    E.Cast();
                }
            }

            if (HarassMode)
            {
                if (Q.CastCheck(Target, "Harass.Q"))
                {
                    Q.Cast();
                }

                var ally = Helpers.AllyBelowHp(ConfigValue<Slider>("Harass.E.Health").Value, E.Range);
                if (E.CastCheck(ally, "Harass.E", true, false))
                {
                    E.Cast();
                }
            }
        }

        public override void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.Sender.IsAlly)
            {
                return;
            }

            if (W.CastCheck(gapcloser.Sender, "Gapcloser.W"))
            {
                W.CastOnUnit(gapcloser.Sender);
            }
        }

        public override void OnPossibleToInterrupt(Obj_AI_Hero target, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (args.DangerLevel < Interrupter2.DangerLevel.High || target.IsAlly)
            {
                return;
            }

            if (Q.CastCheck(target, "Interrupt.Q"))
            {
                Q.Cast();
            }

            if (W.CastCheck(target, "Interrupt.W"))
            {
                W.CastOnUnit(target);
            }
        }

        public override void ComboMenu(Menu config)
        {
            config.AddBool("Combo.Q", "使用 Q", true);
            config.AddBool("Combo.W", "使用 WQ", true);
            config.AddBool("Combo.E", "使用 E", true);
            config.AddSlider("Combo.E.Health", "健康恢复", 20, 1, 100);
        }

        public override void HarassMenu(Menu config)
        {
            config.AddBool("Harass.Q", "使用 Q", true);
            config.AddBool("Harass.E", "使用 E", true);
            config.AddSlider("Harass.E.Health", "健康恢复", 20, 1, 100);
        }

        public override void InterruptMenu(Menu config)
        {
            config.AddBool("Gapcloser.W", "使用 W 防止突进", true);

            config.AddBool("Interrupt.Q", "使用 Q 打断技能", true);
            config.AddBool("Interrupt.W", "使用 W 打断技能", true);
        }
    }
}