import { memo } from "react";

function MovieCard({ movie }) {
  const placeholder = "https://via.placeholder.com/200x300?text=Sin+imagen";

  return (
    <article className="movie-card">
      <img
        src={movie.Poster !== "N/A" ? movie.Poster : placeholder}
        alt={movie.Title}
      />
      <div className="movie-info">
        <h3>{movie.Title}</h3>
        <p className="movie-year">{movie.Year}</p>
        <p className="movie-type">{movie.Type}</p>
      </div>
    </article>
  );
}

export default memo(MovieCard);
