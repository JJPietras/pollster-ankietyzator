import {PipeTransform, Pipe } from '@angular/core'

@Pipe({
    name: 'pollFilter'
})

export class PollsAdminPanelPipe implements PipeTransform{
    
    transform(polls: PollStats[], searchTerm: string): PollStats[] {

        if(!polls || !searchTerm){
          return polls;
        }
        
        return polls.filter(poll =>
          poll.title.toLowerCase().indexOf(searchTerm.toLowerCase()) !== -1);
      }
}