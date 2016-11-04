// To-do:
//  - Rewrite old system to non-crashing version.

// ---
// Packages
// ---

deactivatePackage(RPG_DamageSystem);

package RPG_DamageSystem
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
			return parent::Damage(%this,%obj,%sourceObject,%position,0,%damageType);
		} else
		{
			return parent::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType);
		}
	}
};

activatePackage(RPG_DamageSystem);

// ---
// Core Functions
// ---

function RPG_DamagePlayer(%client, %damage, %type)
{
	//reduce the damage a player takes (if the buff is there)
	if(%client.player.RPG_DamageReductionTimes != 0)
	{
		%damage -= %damage * (%client.player.RPG_DamageReduction / 100);
		%client.player.RPG_DamageReductionTimes--; //isnt timer based, will reduce damage for x amount of hits

		if(%client.player.RPG_DamageReductionTimes == 0)
		{
			%client.player.RPG_DamageReduction = 0;
		}
	}

	//increase the damage a player takes (if the debuff is there)
	if(%client.player.RPG_DamageMultiplierTimes != 0)
	{
		%damage -= %damage * (%client.player.RPG_DamageMultiplier / 100);
		%client.player.RPG_DamageMultiplierTimes--; //isnt timer based, will multiply damage for x amount of hits

		if(%client.player.RPG_DamageMultiplierTimes == 0)
		{
			%client.player.RPG_DamageMultiplier = 0;
		}
	}

	if(%client.player.RPG_DamageShield > 0)
	{
		%client.player.RPG_DamageShield -= %damage;

		return;
	}

	%client.player.RPG_Health -= %damage;

	if(%client.player.RPG_Health <= 0)
	{
		%client.chatMessage("\c0You have died!");
	}
}

function RPG_HealPlayer(%client, %heal)
{
	%client.player.RPG_Health += %heal;
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