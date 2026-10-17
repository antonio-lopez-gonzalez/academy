import { useState, useEffect, useCallback } from "react";
import { taskService } from "../services/taskService";

export function usePaginatedTasks(projectId, filters) {
  const [tasks, setTasks] = useState([]);
  const [page, setPage] = useState(1);
  const [pageSize] = useState(10);
  const [total, setTotal] = useState(0);
  const [loading, setLoading] = useState(true);

  async function load() {
    try {
      setLoading(true);
      const data = await taskService.search(projectId, {
        ...filters,
        page,
        pageSize,
      });
      setTasks(data.items);
      setTotal(data.total);
    } catch (err) {
      console.error(err);
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    setPage(1);
  }, [filters]);

  useEffect(() => {
    load();
  }, [projectId, filters, page, pageSize]);

  const nextPage = useCallback(() => setPage((p) => p + 1), []);
  const prevPage = useCallback(() => setPage((p) => Math.max(1, p - 1)), []);
  const totalPages = Math.ceil(total / pageSize) || 1;

  return { tasks, loading, page, totalPages, nextPage, prevPage, reload: load };
}
