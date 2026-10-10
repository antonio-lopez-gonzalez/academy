import MovieCard from "./MovieCard";

export default function MovieList({ movies }) {
  if (!movies || movies.length === 0) {
    return <p className="no-results">No se encontraron películas</p>;
  }

  return (
    <div className="movie-grid">
      {movies.map((movie) => (
        <MovieCard key={movie.imdbID} movie={movie} />
      ))}
    </div>
  );
}
