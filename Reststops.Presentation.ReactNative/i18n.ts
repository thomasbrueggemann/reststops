import i18n from "i18next";
import { initReactI18next } from "react-i18next";

import * as de from "./locales/de.json";
import * as en from "./locales/en.json";

i18n.use(initReactI18next)
	.init({
		lng: "en",
		defaultNS: "common",
		debug: false,
		initImmediate: true,
		interpolation: {
			escapeValue: false,
		},
		react: { 
			useSuspense: false
		}
	});

i18n.addResourceBundle("de", "common", de);
i18n.addResourceBundle("en", "common", en);

export default i18n;
