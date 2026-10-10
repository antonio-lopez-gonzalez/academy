// Etiqueta de habilidad: nombre + nivel en puntos (●●○)
export default function SkillTag({ name, level = 1 }) {
  // level: 1-3. Pintamos tantos puntos llenos como nivel.
  const dots = "●".repeat(level) + "○".repeat(3 - level);

  return (
    <span className="skill-tag">
      {name} <span className="skill-tag__dots">{dots}</span>
    </span>
  );
}
