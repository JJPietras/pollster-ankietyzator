interface PollStats {
  pollId: number;
  completions: number;
  percentage: number;


  authorId: number;
  authorEmail: string;
  authorName: string;
  title: string;
  description: string;
  tags: string;
  emails: string;
  nonAnonymous: boolean;
  archived: boolean;
  questions: Question [];
}
