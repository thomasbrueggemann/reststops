import i18n from "i18next";
import { initReactI18next } from "react-i18next";
import { ENABLED_LOCALES } from "./constants";

i18n.use(initReactI18next)
	.init({
		lng: "en",
		defaultNS: "common",
		debug: false,
		initImmediate: true,
		interpolation: {
			escapeValue: false,
		}
	});

for (const lang of ENABLED_LOCALES) {
	i18n.addResourceBundle(lang, "common", require(`./locales/${lang}.json`));
}

export default i18n;
