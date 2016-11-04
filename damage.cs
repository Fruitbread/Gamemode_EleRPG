// To-do:
//  - Make all this work with stats

new ScriptObject(EleRPG_Damage) { };

function EleRPG_Damage::getStats(%this, %client)
{
	return %client.EleRPG_stats;
}

deactivatePackage(EleRPG_DamagePackage);

package EleRPG_DamagePackage
{
	function armor::onAdd(%data, %player)
	{
		parent::onAdd(%data, %player);

		if(!isObject(EleRPG_Damage.getStats(%player.client)))
			EleRPG_Stats.createNewStats(%player.client);

		%obj = %player.client.EleRPG_stats;

		%player.RPG_MaxHealth = 10 * %obj.statEndurance[%obj.statLevel];
		%player.RPG_Health = %player.RPG_MaxHealth;

		%player.RPG_MaxMana = 10 * %obj.statMagic[%obj.statLevel];
		%player.RPG_Mana = %player.RPG_MaxMana;
	}

	function GameConnection::spawnPlayer(%client)
	{
		%player = %client.player;

		if(!%client.EleRPG_stats)
			EleRPG_Stats.createNewStats(%client);

		%player.RPG_MaxHealth = 10 * %client.EleRPG_stats.statEndurance[%client.EleRPG_stats.statLevel];
		%player.RPG_Health = %player.RPG_MaxHealth;

		%player.RPG_MaxMana = 10 * %client.EleRPG_stats.statMagic[%client.EleRPG_stats.statLevel];
		%player.RPG_Mana = %player.RPG_MaxMana;

		return parent::spawnPlayer(%client);
	}

	function armor::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType)
	{
		if($Server::RPG::Running) // This probably isn't the best solution, but it works for now
		{
		  EleRPG_DamagePlayer(%obj.client, %damage, %damageType);

			return parent::Damage(%this,%obj,%sourceObject,%position,0,%damageType);
		} else
		{
			return parent::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType);
		}
	}
};

activatePackage(EleRPG_DamagePackage);

// ---
// Main Functionalities
// ---

function EleRPG_Damage::DamagePlayer(%this, %client, %amount, %type)
{
  %player = %client.player;

  //reduce phys damage based on strength
  if(%type $= "physical")
  {
    %strength = %client.EleRPG_stats.statStrength[%client.EleRPG_stats.statLevel];

    %amount = %amount - ((%amount/(1501-%strength)));
  }
  //reduce magic damage based on magic
  if(%type $= "magic")
  {
    %magic = %client.EleRPG_stats.statMagic[%client.EleRPG_stats.statLevel];

    %amount = %amount - ((%amount/(1501-%magic)));
  }

	%amount = mFloor(%amount);

	if(%client.player.RPG_DamageShield > 0)
	{
		if(%amount > %client.player.RPG_DamageShield)
		{
			%temp = %client.player.RPG_DamageShield;
			%client.player.RPG_DamageShield -= %amount;
			%amount -= %temp;

			if(%client.player.RPG_Health <= 0)
			{
				%client.chatMessage("\c0You have died!");
				//soon will punish for death, for now just set health back to max

				%client.player.RPG_Health = %client.player.RPG_MaxHealth;
			}

		} else
		{
			%client.player.RPG_DamageShield -= %amount;
			%amount = 0;
		}

		if(%client.player.RPG_DamageShield <= 0)
			%client.player.RPG_DamageShield = 0;

		%this.displayBottomPrint(%client);
		return;
	}

	%client.player.RPG_Health -= %amount;

	if(%client.player.RPG_Health <= 0)
	{
		%client.chatMessage("\c0You have died!");
		//soon will punish for death, for now just set health back to max

		%client.player.RPG_Health = %client.player.RPG_MaxHealth;
	}

  %this.displayBottomPrint(%client);
}

function EleRPG_Damage::HealPlayer(%this, %client, %amount)
{
	%amount = mFloor(%amount);

	%client.player.RPG_Health += %amount;
	if(%client.player.RPG_Health > %client.player.RPG_MaxHealth)
		%client.player.RPG_Health = %client.player.RPG_MaxHealth;

	%this.displayBottomPrint(%client);
}

function EleRPG_Damage::RemoveMana(%this, %client, %amount)
{
	%amount = mFloor(%amount);

	%client.player.RPG_Mana -= %amount;

	if(%client.player.RPG_Mana <= 0)
		%client.player.mana = 0;

	%this.displayBottomPrint(%client);
}

function EleRPG_Damage::IncreaseMana(%this, %client, %amount)
{
	%amount = mFloor(%amount);

	%client.player.RPG_Mana += %amount;

	if(%client.player.RPG_Mana >= %client.player.RPG_MaxMana)
		%client.player.mana = %client.player.RPG_MaxMana;

	%this.displayBottomPrint(%client);
}

function EleRPG_Damage::setDamageShield(%this, %client, %amount)
{
	%client.player.RPG_DamageShield = %amount;
	%this.displayBottomPrint(%client);
}

// ---
// DISPLAY STUFF
// ---

function createHealthBars(%max, %curr, %shield)
{
    %numColoredBars = mFloor(%curr/%max*20);
    %bars = "\c7";
    %i = 0;
    for (%i = 0; %i < 20 - %numColoredBars; %i++)
        %bars = %bars @ "|";

		if(%shield > 0)
		{
			%shieldBars = mFloor(%shield/%max*20);
			if (%i < %shieldBars)
	    {
	        %bars = %bars @ "\c4";
					%x = %i;
	        for (%j = %i; %j < (%x + %shieldBars); %j++)
	        {
	            %bars = %bars @ "|";
	            %i += 1;
	        }
	    }
		}

    if (%i < 20)
    {
        %bars = %bars @ "\c0";
        for (%j = %i; %j < 20; %j++)
        {
            %bars = %bars @ "|";
            %i += 1;
        }
    }
    return %bars;
}

function createManaBars(%max, %curr)
{
    %numColoredBars = mFloor(%curr/%max*20);
    %bars = "\c7";
    %i = 0;
    for (%i = 0; %i < 20 - %numColoredBars; %i++)
        %bars = %bars @ "|";

    if (%i < 20)
    {
        %bars = %bars @ "\c5";
        for (%j = %i; %j < 20; %j++)
        {
            %bars = %bars @ "|";
            %i += 1;
        }
    }
    return %bars;
}

function EleRPG_Damage::displayBottomPrint(%this, %client)
{
	%health = %client.player.RPG_Health + %client.player.RPG_DamageShield;
	if(%client.player.RPG_DamageShield > 0)
	{
		%health = "\c4" @ %health;
	}

	%healthText = "<font:Tahoma Bold:18>  \c0HP: " @ %health @ "\c7/\c0" @ %client.player.RPG_MaxHealth;
	%healthBar = "<font:Arial Bold:30>" SPC createHealthBars(%client.player.RPG_MaxHealth, %client.player.RPG_Health, %client.player.RPG_DamageShield);

	//%shieldText = "<font:Tahoma:15>\c4" @ %client.player.RPG_DamageShield;

	%manaText = "<font:Tahoma Bold:19>\c5MP: " @ %client.player.RPG_Mana @ "\c7/\c5" @ %client.player.RPG_MaxMana;
	%manaBar = "<font:Arial Bold:30>" SPC createManaBars(%client.player.RPG_MaxMana, %client.player.RPG_Mana);

	%client.bottomprint("<just:left>" @ %healthText @ "<just:right>" @ %manaText @ "<br><just:left>" @ %healthBar @ "<just:right>" @ %manaBar @ "<br>", 100, 1);
}

function EleRPG_Damage::displayTick(%this)
{
	cancel($EleRPG::DisplayTick);
	%this.updateClientDisplay();
	$EleRPG::DisplayTick = %this.schedule(1000, displayTick);
}

function EleRPG_Damage::updateClientDisplay(%this)
{
	for(%i = 0; %i < ClientGroup.getCount(); %i++)
	{
		%client = ClientGroup.getObject(%i);
		%this.displayBottomPrint(%client);
	}
}

EleRPG_Damage.displayTick();
