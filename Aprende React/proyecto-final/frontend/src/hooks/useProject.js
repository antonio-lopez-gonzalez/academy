import { useState, useEffect, useCallback } from "react";
import { projectService } from "../services/projectService";

export function useProject(id) {
  const [project, setProject] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  async function loadProject() {
    try {
      setLoading(true);
      const data = await projectService.getById(id);
      setProject(data);
      setError(null);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    loadProject();
  }, [id]);

  const updateProject = useCallback(
    async (updates) => {
      const updated = await projectService.update(id, updates);
      setProject(updated);
      return updated;
    },
    [id]
  );

  const deleteProject = useCallback(async () => {
    await projectService.delete(id);
  }, [id]);

  return {
    project,
    loading,
    error,
    updateProject,
    deleteProject,
    reload: loadProject,
  };
}
