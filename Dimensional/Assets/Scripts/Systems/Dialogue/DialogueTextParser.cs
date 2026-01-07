using System.Collections.Generic;
using Scriptables.Dialogue;

namespace Systems.Dialogue
{
    public static class DialogueTextParser
    {
        public static List<DialogueLine> Parse(string text, DialogueSpeakerDatum[] speakers)
        {
            List<DialogueLine> lines = new List<DialogueLine>();

            string[] rawLines = text.Split('\n');
            int i = 0;

            while (i < rawLines.Length)
            {
                string header = rawLines[i].Trim();

                // Skip empty lines
                if (string.IsNullOrWhiteSpace(header))
                {
                    i++;
                    continue;
                }

                // Parse header: "0|emotion=happy;anim=wave"
                string[] headerParts = header.Split('|');
                int speakerIndex = int.Parse(headerParts[0]);

                string metadata = headerParts.Length > 1 ? headerParts[1] : "";

                // Dialogue text is always the next line
                i++;
                if (i >= rawLines.Length)
                    break;

                string dialogueText = rawLines[i].Trim();

                DialogueLine line = new DialogueLine();
                
                /*
                DialogueLine line = new DialogueLine(
                    speakers[speakerIndex],
                    dialogueText,
                    metadata
                );
                */

                lines.Add(line);

                i++;
            }

            return lines;
        }
    }
}