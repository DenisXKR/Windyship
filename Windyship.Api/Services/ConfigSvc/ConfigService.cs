using System.Configuration;
using System.Web.Configuration;

namespace Windyship.Api.Services.ConfigSvc
{
	public sealed class ConfigService : IConfigService
	{
		public T GetRequiredSection<T>(string sectionName) where T : ConfigurationSection
		{
			var section = WebConfigurationManager.GetSection(sectionName) as T;
			if (section == null)
			{
				throw new ConfigurationErrorsException(string.Format("Required configuration section '{0}' of type '{1}' is absent.", sectionName, typeof (T).Name));
			}

			return section;
		}

		public int GetRequiredAppSettingInt(string name)
		{
			var valueStr = WebConfigurationManager.AppSettings.Get(name);

			int value;
			if (!int.TryParse(valueStr, out value))
			{
				throw new ConfigurationErrorsException(string.Format("App setting with name '{0}' must present and must be int type. Current value = '{1}'", name, valueStr));
			}

			return value;
		}

		public string GetRequiredAppSettingString(string name)
		{
			return WebConfigurationManager.AppSettings.Get( name );
		}
	}
}