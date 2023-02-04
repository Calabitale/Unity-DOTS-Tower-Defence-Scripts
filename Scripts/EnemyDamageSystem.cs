using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

//[UpdateAfter(typeof(TurretAttackSystem))]
public partial class PlayerDamageSystem : SystemBase
{
    protected override void OnUpdate()
    {

        var PlayerSouls = HasSingleton<ProfitsDat>() ? GetSingleton<ProfitsDat>() : default;

        Entities.
            //WithStructuralChanges().
            //WithReadOnly(BestDirectionCells).
            //WithReadOnly(TranslutionEnts). 
            //WithDisposeOnCompletion(TranslutionEnts).
            WithAll<EnemyDudeTag>().
            ForEach((Entity medude, ref DamageTaken enemdamage, ref CurrHealth curryheath, ref IsAlive alive, ref EnemyStatesDat mestate) =>
            {
                if (enemdamage.intVal == 0 && curryheath.intVal != 0)//
                    return;               

                curryheath.intVal -= enemdamage.intVal;

                if (curryheath.intVal <= 0)
                {
                    mestate.EnemyState = EnemyStates.leavingentrance;
                    //alive.booVal = false;                  
                    //PlayerSouls.intVal += bassheath.intVal;

                }

                enemdamage.intVal = 0;

            }).Run();

        //SetSingleton<Profits>(PlayerSouls);

    }
}
