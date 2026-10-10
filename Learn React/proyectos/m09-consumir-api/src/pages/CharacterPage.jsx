import { useState, useEffect } from "react";
import { useParams, Link } from "react-router-dom";
import { characterService } from "../services/characterService";

export default function CharacterPage() {
  const { id } = useParams();
  const [character, setCharacter] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const controller = new AbortController();

    async function loadCharacter() {
      setLoading(true);
      setError(null);
      try {
        const data = await characterService.getById(id);
        setCharacter(data);
      } catch (err) {
        if (err.name !== "AbortError") {
          setError(err.message);
        }
      } finally {
        setLoading(false);
      }
    }

    loadCharacter();

    return () => controller.abort();
  }, [id]);

  if (loading) return <p>Cargando personaje...</p>;
  if (error) return <p style={{ color: "red" }}>Error: {error}</p>;
  if (!character) return <p>Personaje no encontrado</p>;

  return (
    <div className="character-detail">
      <Link to="/">← Volver</Link>
      <img src={character.image} alt={character.name} />
      <h1>{character.name}</h1>
      <p><strong>Estado:</strong> {character.status}</p>
      <p><strong>Especie:</strong> {character.species}</p>
      <p><strong>Género:</strong> {character.gender}</p>
      <p><strong>Origen:</strong> {character.origin.name}</p>
      <p><strong>Ubicación:</strong> {character.location.name}</p>
    </div>
  );
}
