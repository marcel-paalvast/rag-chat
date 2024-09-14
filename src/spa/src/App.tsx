import './App.css'

import Api from './api/api';
import ApiSettings from './api/apiSettings';
import { createContext, useState } from 'react';
import Assistant from './models/assistant';
import Selector from './components/Selector';
import Chat from './components/Chat';

const apiConfig: ApiSettings = {
  baseUri: import.meta.env.VITE_API_BASEURI,
  apiKey: import.meta.env.VITE_API_KEY,
};

const api = new Api(apiConfig);

export const ApiContext = createContext(api);

function App() {
  const [assistant, setAssistant] = useState<Assistant>();

  return (
    <>
      {!assistant && <Selector onSelect={setAssistant} />}
      <Chat assistant={assistant} onClose={() => setAssistant(undefined)} />
    </>
  );
}

export default App
