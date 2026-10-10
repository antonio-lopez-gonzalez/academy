import { useState, useEffect } from "react";
import { characterService } from "../services/characterService";
import CharacterCard from "../components/CharacterCard";
import FilterPanel from "../components/FilterPanel";
import Pagination from "../components/Pagination";

export default function Home() {
  const [characters, setCharacters] = useState([]);
  const [info, setInfo] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [currentPage, setCurrentPage] = useState(1);
  const [filters, setFilters] = useState({ status: "", gender: "" });
  const [search, setSearch] = useState("");

  useEffect(() => {
    const controller = new AbortController();

    async function loadCharacters() {
      setLoading(true);
      setError(null);
      try {
        const params = { page: currentPage };
        if (filters.status) params.status = filters.status;
        if (filters.gender) params.gender = filters.gender;
        if (search) params.name = search;

        const data = await characterService.getAll(params);
        setCharacters(data.results);
        setInfo(data.info);
      } catch (err) {
        if (err.name !== "AbortError") {
          setError(err.message);
        }
      } finally {
        setLoading(false);
      }
    }

    loadCharacters();

    return () => controller.abort();
  }, [currentPage, filters, search]);

  return (
    <div>
      <h1>Rick and Morty Explorer</h1>

      <input
        type="search"
        placeholder="Buscar personaje..."
        value={search}
        onChange={(e) => {
          setSearch(e.target.value);
          setCurrentPage(1);
        }}
      />

      <FilterPanel filters={filters} onFilterChange={setFilters} />

      {loading && <p>Cargando personajes...</p>}
      {error && <p style={{ color: "red" }}>Error: {error}</p>}

      {!loading && !error && (
        <>
          <div className="character-grid">
            {characters.map((character) => (
              <CharacterCard key={character.id} character={character} />
            ))}
          </div>
          <Pagination
            info={info}
            currentPage={currentPage}
            onPageChange={setCurrentPage}
          />
        </>
      )}
    </div>
  );
}
