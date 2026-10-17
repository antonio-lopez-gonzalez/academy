import SkillTag from "./SkillTag.jsx";

// Lista de habilidades: un SkillTag por skill
export default function SkillList({ skills }) {
  return (
    <div className="skill-list">
      {skills.map((skill) => (
        <SkillTag key={skill.name} name={skill.name} level={skill.level} />
      ))}
    </div>
  );
}
