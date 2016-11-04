// To-do:
// ~ Health Modifiers


//				PLEASE DO NOT USE THIS EVER, IT HAS A GAME CRASHING BUG.
//				If someone dies, it crashes the game. Simple.


// ---
// Package Stuff
// ---

deactivatePackage(RPG_DamageSys);

package RPG_DamageSys
{
	function armor::onAdd(%data, %player)
	{
		parent::onAdd(%data, %player);
		if(%player.client.RPG_Constitution == 0)
			%player.client.RPG_Constitution = 10;
		%player.RPG_MaxHealth = %player.client.RPG_Constitution * 10;
		%player.RPG_Health = %player.client.RPG_Constitution * 10;

		if(%player.client.RPG_Magick == 0)
			%player.client.RPG_Magick = 10;
		%player.RPG_MaxMana = %player.client.RPG_Magick * 10;
		%player.RPG_Mana = %player.client.RPG_Magick * 10;
	}

	function armor::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType)
	{
		if($Server::RPG::Running) // This probably isn't the best solution, but it works for now
		{
			RPG_DamagePlayer(%obj.client, %damage, %damageType);
		} else
		{
			return parent::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType);
		}
	}
};

activatePackage(RPG_DamageSys);

// ---
// Mechanic Functions
// ---

function RPG_DamagePlayer(%client, %damage, %type)
{
	// (De)buff stuff
	if(%client.player.RPG_DamageReduction != 0)
	{
		%damage -= %damage * (%client.player.RPG_DamageReduction / 100);
		%client.player.RPG_DamageReductionTimes--;
		if(%client.player.RPG_DamageReductionTimes == 0)
		{
			%client.player.RPG_DamageReduction = 0;
			%client.player.RPG_DamageReductionTimes = 0;
		}
	}

	if(%client.player.RPG_DamageMultiplier != 0)
	{
		%damage -= %damage * (%client.player.RPG_DamageMultiplier / 100);
		%client.player.RPG_DamageMultiplierTimes--;
		if(%client.player.RPG_DamageMultiplierTimes == 0)
		{
			%client.player.RPG_DamageMultiplier = 0;
			%client.player.RPG_DamageMultiplierTimes = 0;
		}
	}

	if(%client.player.RPG_DamageShield > 0)
	{
		%client.player.RPG_DamageShield -= %damage;

		return;
	}

	%client.player.RPG_Health -= %damage;
	%actualDamage = getDamagePercentage(%client, %damage);
	%client.player.setHealth(getPlayerHealth(%client) - %actualDamage);
}

function RPG_HealPlayer(%client, %heal)
{
	%client.player.RPG_Health += %heal;
	%actualHeal = getDamagePercentage(%client, %heal);
	%client.player.setHealth(getPlayerHealth(%client) + %actualHeal);
}

// ---
// Player Buffs
// ---

function RPG_AddDamageReduction(%client, %amount, %amt)
{
	if(%client.player.RPG_DamageReduction != 0)
		return;

	%client.player.RPG_DamageReduction = %amount;
	%client.player.RPG_DamageReductionTimes = %amt;
}

function RPG_AddDamageShield(%client, %amount)
{
	%client.player.RPG_DamageShield = %amount;
}

// ---
// Player Debuffs
// ---

function RPG_AddDamageMultiplier(%client, %amount, %amt)
{
	if(%client.player.RPG_DamageMultiplier != 0)
		return;

	%client.player.RPG_DamageMultiplier = %amount;
	%client.player.RPG_DamageMultiplierTimes = %amt;
}

// ---
// Server Commands
// ---

function serverCmdHealth(%client)
{
	%client.chatMessage("\c6---");
	%client.chatMessage("\c7[RPG] \c0Health: \c6" @ %client.player.RPG_Health @ " / " @ %client.player.RPG_MaxHealth);
	if(%client.player.RPG_DamageShield != 0)
		%client.chatMessage("\c7[RPG] \c5Shield: \c6" @ %client.player.RPG_DamageShield);
	%client.chatMessage("\c7[RPG] \c1Magick: \c6" @ %client.player.RPG_Mana @ " / " @ %client.player.RPG_MaxMana);
}

function serverCmdH(%client)
{
	serverCmdHealth(%client);
}