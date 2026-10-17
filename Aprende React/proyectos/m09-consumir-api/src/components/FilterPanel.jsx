export default function FilterPanel({ filters, onFilterChange }) {
  return (
    <div className="filter-panel">
      <div>
        <label>Estado:</label>
        <select
          value={filters.status}
          onChange={(e) => onFilterChange({ ...filters, status: e.target.value })}
        >
          <option value="">Todos</option>
          <option value="alive">Vivo</option>
          <option value="dead">Muerto</option>
          <option value="unknown">Desconocido</option>
        </select>
      </div>

      <div>
        <label>Género:</label>
        <select
          value={filters.gender}
          onChange={(e) => onFilterChange({ ...filters, gender: e.target.value })}
        >
          <option value="">Todos</option>
          <option value="female">Femenino</option>
          <option value="male">Masculino</option>
          <option value="genderless">Sin género</option>
          <option value="unknown">Desconocido</option>
        </select>
      </div>
    </div>
  );
}
