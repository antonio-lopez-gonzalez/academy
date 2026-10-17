import { useState, useMemo } from "react";
import { useDebounce } from "./hooks/useDebounce";
import { useFetch } from "./hooks/useFetch";
import SearchInput from "./components/SearchInput";
import MovieList from "./components/MovieList";

const API_KEY = "tu_api_key_aqui";

export default function App() {
  const [query, setQuery] = useState("");
  const debouncedQuery = useDebounce(query, 500);

  const url = debouncedQuery
    ? `https://www.omdbapi.com/?s=${encodeURIComponent(debouncedQuery)}&apikey=${API_KEY}`
    : "";

  const { data, loading, error } = useFetch(url);

  const movies = useMemo(() => data?.Search || [], [data]);

  return (
    <div className="app">
      <header>
        <h1>🎬 Buscador de Películas</h1>
        <p className="subtitle">Powered by OMDb API</p>
      </header>

      <SearchInput value={query} onChange={setQuery} />

      <div className="status">
        {loading && <p className="loading">Buscando…</p>}
        {error && <p className="error">⚠️ {error}</p>}
      </div>

      {!loading && !error && debouncedQuery && <MovieList movies={movies} />}

      {!debouncedQuery && (
        <p className="hint">Escribe el nombre de una película para buscar</p>
      )}
    </div>
  );
}
