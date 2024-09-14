import { useContext, useEffect, useState } from 'react';
import { Autocomplete, TextField, Typography, Card, CardContent, Stack, Box, Button, CircularProgress, Snackbar } from '@mui/material';
import Assistant from '../models/assistant';
import { ApiContext } from '../App';
import './Selector.css';

function Selector({ onSelect }: { onSelect: (assistant: Assistant) => void }) {
  const [assistants, setAssistants] = useState<Assistant[]>([]);
  const [categories, setCategories] = useState<string[]>([]);
  const [selectedCategory, setSelectedCategory] = useState<string | null>(null);
  const [selectedAssistant, setSelectedAssistant] = useState<Assistant | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string>();
  const api = useContext(ApiContext);

  useEffect(() => {
    const fetchAssistants = async () => {
      try {
        const assistants = await api.getAssistants();
        const orderedAssistants = assistants.sort((a, b) => {
          const categoryComparison = a.category.localeCompare(b.category);
          if (categoryComparison !== 0) {
            return categoryComparison;
          }
          return a.name.localeCompare(b.name);
        });
        setAssistants(orderedAssistants);

        const categories = [...new Set(assistants.map(assistant => assistant.category))];
        setCategories(categories);
      } catch (error) {
        if (error instanceof Error) {
          setError(error.message);
        }
      }

      setLoading(false);
    };

    fetchAssistants();
  }, [api]);

  const handleCategoryChange = (_event: React.SyntheticEvent, value: string | null) => {
    setSelectedCategory(value);
    setSelectedAssistant(null); // Reset assistant
  };

  const handleAssistantChange = (_event: React.SyntheticEvent, value: Assistant | null) => {
    setSelectedAssistant(value);
    if (value) {
      setSelectedCategory(value.category);
    }
  };
  return (
    <>
      <Box className="glass-effect">
        <Card sx={{ minWidth: 512, textAlign: 'center' }}>
          <CardContent>
            <Typography variant="h5" gutterBottom color="primary">
              Select an Assistant
            </Typography>
            <Stack spacing={3}>
              <Typography variant="body2" gutterBottom>
                To start chatting select an assistant.
                <br />
                The topic and assistant will affect their expertise and responses.
              </Typography>
              {loading ? (
                <Box display="flex" justifyContent="center" alignItems="center" height="100px">
                  <CircularProgress />
                </Box>
              ) : (
                <>
                  <Autocomplete
                    options={categories}
                    value={selectedCategory}
                    onChange={handleCategoryChange}
                    renderInput={(params) => <TextField {...params} label="Select Topic" />}
                  />
                  <Autocomplete
                    options={
                      selectedCategory
                        ? assistants.filter((assistant) => assistant.category === selectedCategory)
                        : assistants
                    }
                    groupBy={(option) => option.category}
                    getOptionLabel={(option) => option.name}
                    value={selectedAssistant}
                    onChange={handleAssistantChange}
                    renderInput={(params) => <TextField {...params} label="Select Assistant" />}
                  />
                </>
              )}
              <Button
                variant="contained"
                color="primary"
                disabled={!selectedAssistant}
                onClick={() => selectedAssistant && onSelect(selectedAssistant)}
              >
                Let's Chat
              </Button>
            </Stack>
          </CardContent>
        </Card>
      </Box>
      <Snackbar
        open={error !== undefined}
        autoHideDuration={5000}
        message={error}
      />
    </>
  );
}

export default Selector;