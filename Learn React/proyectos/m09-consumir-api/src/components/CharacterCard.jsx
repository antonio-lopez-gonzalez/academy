import { Link } from "react-router-dom";

export default function CharacterCard({ character }) {
  const statusColor = {
    Alive: "#4caf50",
    Dead: "#f44336",
    unknown: "#9e9e9e",
  };

  return (
    <Link to={`/character/${character.id}`} className="character-card">
      <img src={character.image} alt={character.name} />
      <div className="character-info">
        <h3>{character.name}</h3>
        <p>
          <span
            className="status-dot"
            style={{ background: statusColor[character.status] }}
          />
          {character.status} - {character.species}
        </p>
      </div>
    </Link>
  );
}
