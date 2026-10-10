import { useState } from "react";

export default function CheckboxRadioSelectDemo() {
  const [newsletter, setNewsletter] = useState(false);
  const [theme, setTheme] = useState("dark");
  const [language, setLanguage] = useState("es");

  return (
    <div>
      <label>
        <input
          type="checkbox"
          checked={newsletter}
          onChange={(e) => setNewsletter(e.target.checked)}
        />
        Suscribirme al newsletter
      </label>

      <fieldset>
        <legend>Tema</legend>
        {["light", "dark", "system"].map((t) => (
          <label key={t}>
            <input
              type="radio"
              name="theme"
              value={t}
              checked={theme === t}
              onChange={(e) => setTheme(e.target.value)}
            />
            {t}
          </label>
        ))}
      </fieldset>

      <select value={language} onChange={(e) => setLanguage(e.target.value)}>
        <option value="es">Español</option>
        <option value="en">English</option>
        <option value="fr">Français</option>
      </select>

      <pre>{JSON.stringify({ newsletter, theme, language }, null, 2)}</pre>
    </div>
  );
}
