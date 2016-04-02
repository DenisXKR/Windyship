using System.Configuration;

namespace Windyship.Api.Services
{
	public interface IConfigService
	{
		T GetRequiredSection<T>(string sectionName) where T : ConfigurationSection;
		int GetRequiredAppSettingInt(string name);

		string GetRequiredAppSettingString(string name);
	}
}