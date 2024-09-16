import Assistant from '../models/assistant'
import { useCallback, useContext, useEffect, useMemo, useRef, useState } from 'react';
import { ApiContext } from '../App';
import ClockProgress from './ClockProgress';
import { Box, Button, IconButton, Paper, SxProps, TextField, Theme, Tooltip, Typography } from '@mui/material';
import ChangeUser from './ChangeUser';

enum User {
  Assistant,
  User,
  Instruction,
  Error,
}

interface Message {
  text: string;
  user: User;
}

function Chat({ assistant, onClose }: { assistant: Assistant | undefined, onClose: () => void }) {
  const api = useContext(ApiContext);
  const [conversationId, setConversationId] = useState<string>();
  const [userMessages, setUserMessages] = useState<Message[]>([]);
  const [inputValue, setInputValue] = useState<string>('');
  const messagesEndRef = useRef<HTMLDivElement>(null);
  const [loading, setLoading] = useState(false);

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  };

  useEffect(() => {
    if (userMessages.length > 0) {
      scrollToBottom();
    }
  }, [userMessages]);

  const sendMessage = async (message: string) => {
    if (!assistant) {
      return;
    }
    if (inputValue.trim() === '') return;

    const nextMessages = [...userMessages, { text: message, user: User.User }];
    setUserMessages(nextMessages);
    setLoading(true);
    setInputValue('');

    try {
      const response = await api.getAssistantChat(
        assistant.id,
        inputValue,
        conversationId,
      );

      const reader = response.reader;
      const decoder = new TextDecoder();

      let text = '';
      const read = async () => {
        const { done, value } = await reader.read();
        if (done) {
          return;
        }
        // Decode the chunk and update the state to display it
        const newText = decoder.decode(value, { stream: true });
        text += newText;

        setUserMessages([...nextMessages, { text: removeMarkdown(text), user: User.Assistant }]);
        // Read the next chunk
        read();
      };

      read();

      if (response.continuationToken) {
        setConversationId(response.continuationToken);
      }
    }
    catch (error: unknown) {
      if (error instanceof Error) {
        setUserMessages([...nextMessages, { text: removeMarkdown(error.message), user: User.Error }]);
      }
    }

    setLoading(false);
  };

  const removeMarkdown = (text: string) => {
    //remove blod markdown
    text = text.replace(/\*\*(.+?)\*\*/g, '$1');
    //remove headers markdown
    text = text.replace(/#+/g, '');
    //remove italic markdown
    text = text.replace(/\*(.+?)\*/g, '$1');
    //remove hyperlink markdown
    text = text.replace(/\[(.+?)\]\((.+?)\)/g, '$1');

    return text;
  }

  const getStyle = (user: User): SxProps<Theme> => {
    const base: SxProps<Theme> = {
      borderRadius: '5px',
      padding: '10px',
      margin: '5px 0',
      display: 'inline-block',
      maxWidth: '80%',
      whiteSpace: 'pre-wrap',
      overflowWrap: 'break-word',
      textAlign: 'left',
      color: '#fff',
    };
    switch (user) {
      case User.Assistant:
        return {
          ...base,
          backgroundColor: '#9500ae',
          alignSelf: 'flex-start',
        };
      case User.User:
        return {
          ...base,
          backgroundColor: '#1769aa',
          alignSelf: 'flex-end',
          textAlign: 'right',
        };
      case User.Instruction:
        return {
          ...base,
          backgroundColor: '#14a37f',
          alignSelf: 'flex-start',
        };
      case User.Error:
        return {
          ...base,
          backgroundColor: 'linear-gradient(130deg,#A11425,#942157)',
          alignSelf: 'flex-start',
        };
      default:
        return {
          ...base,
          backgroundColor: '#b28900',
          alignSelf: 'flex-start',
        };
    }
  };

  const allMessages = useMemo(() => [
    {
      text: `Welcome! I'm '${assistant?.name}'. How can I help you today?`,
      user: User.Instruction,
    },
    ...userMessages,
  ], [assistant?.name, userMessages]);

  const closeChat = useCallback(() => {
    setUserMessages([]);
    onClose();
  }, [onClose, setUserMessages]);

  return (
    <Box
      className="chat"
      sx={{
        display: 'flex',
        flexDirection: 'column',
        width: '1024px',
        height: '100vh',
        padding: '10px',
        boxSizing: 'border-box',
      }}
    >
      <Box
        className='change-assistant'
        sx={{
          position: 'relative',
          width: '100%',
        }}
      >
        <Tooltip title='Change Assistant' placement='bottom'>
          <IconButton
            sx={{
              position: 'absolute',
              top: '5px',
              right: '5px',
            }}
            onClick={closeChat}
          >
            <ChangeUser />
          </IconButton>
        </Tooltip>
      </Box>
      <Box
        className='chat-messages'
        sx={{
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'flex-start',
          overflowY: 'scroll',
          height: '100%',
          flex: 1,
          scrollbarWidth: 'none',
          msOverflowStyle: 'none',
        }}
      >
        {allMessages.map((message, index) => (
          <Paper
            key={index}
            sx={getStyle(message.user)}
          >
            <Typography sx={{ alignSelf: 'inherit' }}>{message.text}</Typography>
          </Paper>
        ))}
        {loading && (
          <Paper sx={getStyle(User.Instruction)}>
            <ClockProgress />
          </Paper>
        )}
        <Box ref={messagesEndRef} sx={{ flexShrink: 0 }} />
      </Box>
      <Box
        sx={{
          margin: '20px 0 20px 0',
          display: 'flex',
          gap: '10px',
        }}
      >
        <TextField
          label='Type your message here'
          color='primary'
          placeholder="Could you help me with..."
          value={inputValue}
          size='small'
          onChange={(e) => setInputValue((e.target as HTMLInputElement).value)}
          onKeyDown={e => {
            if (e.key === 'Enter') {
              sendMessage(inputValue);
            }
          }}
          sx={{
            flex: 1,
          }}
          slotProps={{
            htmlInput: {
              maxLength: 2048
            }
          }}
        />
        <Button
          variant='contained'
          onClick={() => sendMessage(inputValue)}
          disabled={loading}
        >
          Send
        </Button>
      </Box>
    </Box>
  );
}

export default Chat